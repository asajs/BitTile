using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace BitTile
{
	public static class ColorTriangle
	{
		public static BitmapSource Create(Point[] trianglePoints, Color[] colors, int height, int width)
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					DrawColorTriangle(graphics, trianglePoints, colors);
				}

				image = CreateBitmapSourceFromGdiBitmap(bitmap);
			}
			return image;
		}

		private static void DrawColorTriangle(Graphics gr, Point[] points, Color[] colors)
		{
			GraphicsPath trianglePath = new GraphicsPath();
			trianglePath.AddPolygon(points);
			trianglePath.Flatten();

			using (PathGradientBrush pgb = new PathGradientBrush(trianglePath))
			{
				pgb.CenterColor = medianColor(colors);
				pgb.SurroundColors = colors;
				gr.FillPolygon(pgb, trianglePath.PathPoints);
			}
		}

		private static Color medianColor(Color[] cols)
		{
			int c = cols.Length;
			return Color.FromArgb(cols.Sum(x => x.A) / c, cols.Sum(x => x.R) / c,
				cols.Sum(x => x.G) / c, cols.Sum(x => x.B) / c);
		}

		private static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
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
	}
}
