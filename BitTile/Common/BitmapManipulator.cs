using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace BitTile.Common
{
	static class BitmapManipulator
	{
		public static BitmapSource CreateBitTile(Color[,] colors, int pixelSize, int pixelsHeight, int pixelsWide)
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(pixelsWide * pixelSize, pixelsHeight * pixelSize))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					//graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode
					graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
					DrawBitMap(graphics, colors, pixelSize, pixelsHeight, pixelsWide);
				}
				image = CreateBitmapSourceFromGdiBitmap(bitmap);
			}
			return image;
		}

		public static BitmapSource DeepCopyImage(System.Windows.Controls.Image image)
		{
			return (BitmapSource)image.Source.Clone();
		}

		public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
		{
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			BitmapData bitmapData = bitmap.LockBits(
				rect,
				ImageLockMode.ReadWrite,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			try
			{
				int size = (rect.Width * rect.Height) * 4;
				return BitmapSource.Create(
					bitmap.Width,
					bitmap.Height,
					bitmap.HorizontalResolution,
					bitmap.VerticalResolution,
					PixelFormats.Bgra32,
					null,
					bitmapData.Scan0,
					size,
					bitmapData.Stride);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}

		public static BitmapSource EditTileOfBitmap(BitmapSource source, Color newColor, System.Windows.Point[] points, int pixelSize)
		{
			Bitmap b = GetBitmap(source);

			BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);

			/* GetBitsPerPixel just does a switch on the PixelFormat and returns the number */
			byte bitsPerPixel = Convert.ToByte(Image.GetPixelFormatSize(bData.PixelFormat));

			/*the size of the image in bytes */
			int size = bData.Stride * bData.Height;

			/*Allocate buffer for image*/
			byte[] data = new byte[size];
			int bits = bitsPerPixel / 8;

			/*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
			Marshal.Copy(bData.Scan0, data, 0, size);

			foreach (System.Windows.Point point in points)
			{
				int y = (int)point.Y * pixelSize;
				int x = (int)point.X * pixelSize;
				for (int i = y * bData.Stride; i < (y + pixelSize) * bData.Stride; i += bData.Stride)
				{
					for (int j = x * bits; j < (x + pixelSize) * bits; j += bits)
					{
						data[i + j] = newColor.B;
						data[i + j + 1] = newColor.G;
						data[i + j + 2] = newColor.R;
						data[i + j + 3] = newColor.A;
					}
				}
			}

			/* This override copies the data back into the location specified */
			Marshal.Copy(data, 0, bData.Scan0, data.Length);

			b.UnlockBits(bData);

			return CreateBitmapSourceFromGdiBitmap(b);
		}

		public static Color SampleSingleRegion(BitmapSource source, int y, int x)
		{
			Bitmap bitmap = GetBitmap(source);

			BitmapData bData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			byte bitsPerPixel = Convert.ToByte(Image.GetPixelFormatSize(bData.PixelFormat));

			int size = bData.Stride * bData.Height;

			byte[] data = new byte[size];
			int bits = bitsPerPixel / 8;

			Marshal.Copy(bData.Scan0, data, 0, size);

			Color color = SampleLargeRegion(data, 1, 1, y, x, bData.Stride, bits);

			bitmap.UnlockBits(bData);

			return color;
		}

		public static Color[,] SampleBitmapSource(BitmapSource source, int destHeight, int destWidth)
		{
			Bitmap bitmap = GetBitmap(source);

			BitmapData bData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			byte bitsPerPixel = Convert.ToByte(Image.GetPixelFormatSize(bData.PixelFormat));

			int size = bData.Stride * bData.Height;

			double heightFactor = bData.Height / (double)destHeight;
			double widthFactor = bData.Width / (double)destWidth;

			byte[] data = new byte[size];
			int bits = bitsPerPixel / 8;

			Color[,] colors = new Color[destHeight, destWidth];

			Marshal.Copy(bData.Scan0, data, 0, size);

			if (widthFactor >= 1 && heightFactor >= 1)
			{
				for (int y = 0; y < destHeight; y++)
				{
					for (int x = 0; x < destWidth; x++)
					{
						colors[y, x] = SampleLargeRegion(data, (int)heightFactor, (int)widthFactor, y, x, bData.Stride, bits);
					}
				}
			}
			else
			{
				for (int y = 0; y < destHeight; y++)
				{
					for (int x = 0; x < destWidth; x++)
					{
						colors[y, x] = SampleSmallRegion(data, heightFactor, widthFactor, y, x, bData.Stride, bits);
					}
				}
			}

			bitmap.UnlockBits(bData);

			return colors;
		}

		private static Color[,] SmartBlowUp(byte[] data, double heightFactor, double widthFactor, int incrementY, int incrementX, int imageHeight, int imageWidth)
		{
			Color[,] originalColors = new Color[imageHeight, imageWidth];

			for (int y = 0; y < imageHeight; y++)
			{
				for (int x = 0; x < imageWidth; x++)
				{
					originalColors[y, x] = SampleLargeRegion(data, (int)heightFactor, (int)widthFactor, y, x, incrementY, incrementX);
				}
			}

			return originalColors;
		}

		private static Color SampleSmallRegion(byte[] data, double heightFactor, double widthFactor, int startY, int startX, int incrementY, int incrementX)
		{
			int redSample = 0;
			int greenSample = 0;
			int blueSample = 0;
			int alphaSample = 0;

			int y = (int)Math.Floor(startY * heightFactor) * incrementY;
			int x = (int)Math.Floor(startX * widthFactor) * incrementX;

			blueSample += data[y + x];
			greenSample += data[y + x + 1];
			redSample += data[y + x + 2];
			alphaSample += data[y + x + 3];

			byte r = Convert.ToByte(redSample);
			byte g = Convert.ToByte(greenSample);
			byte b = Convert.ToByte(blueSample);
			byte a = Convert.ToByte(alphaSample);

			Color color = Color.FromArgb(a, r, g, b);
			return color;
		}

		private static Color SampleLargeRegion(byte[] data, int heightFactor, int widthFactor, int startY, int startX, int incrementY, int incrementX)
		{
			int redSample = 0;
			int greenSample = 0;
			int blueSample = 0;
			int alphaSample = 0;
			int startPointY = startY * heightFactor * incrementY;
			int startPointX = startX * widthFactor * incrementX;
			int endPointY = startPointY + heightFactor * incrementY;
			int endPointX = startPointX + widthFactor * incrementX;
			for (int y = startPointY; y < endPointY; y += incrementY)
			{
				for (int x = startPointX; x < endPointX; x += incrementX)
				{
					blueSample += data[y + x];
					greenSample += data[y + x + 1];
					redSample += data[y + x + 2];
					alphaSample += data[y + x + 3];
				}
			}

			int averagingComponent = heightFactor * widthFactor;

			redSample /= averagingComponent;
			greenSample /= averagingComponent;
			blueSample /= averagingComponent;
			alphaSample /= averagingComponent;

			byte r = Convert.ToByte(redSample);
			byte g = Convert.ToByte(greenSample);
			byte b = Convert.ToByte(blueSample);
			byte a = Convert.ToByte(alphaSample);

			Color color = Color.FromArgb(a, r, g, b);
			return color;
		}

		private static Bitmap GetBitmap(BitmapSource source)
		{
			Bitmap bmp = new Bitmap(
			  source.PixelWidth,
			  source.PixelHeight,
			  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData data = bmp.LockBits(
			  new Rectangle(System.Drawing.Point.Empty, bmp.Size),
			  ImageLockMode.WriteOnly,
			  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			source.CopyPixels(
			  Int32Rect.Empty,
			  data.Scan0,
			  data.Height * data.Stride,
			  data.Stride);
			bmp.UnlockBits(data);
			return bmp;
		}

		private static void DrawBitMap(Graphics gr, Color[,] colors, int pixelSize, int pixelsHeight, int pixelsWide)
		{
			for (int i = 0; i < pixelsHeight; i++)
			{
				for (int j = 0; j < pixelsWide; j++)
				{
					SolidBrush brush = new SolidBrush(colors[i, j]);
					gr.FillRectangle(brush, j * pixelSize, i * pixelSize, pixelSize, pixelSize);
				}
			}
		}
	}
}
