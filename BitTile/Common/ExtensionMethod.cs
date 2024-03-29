﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ExtensionMethods
{
	public static class MyExtensions
	{
		public static double AngleToRadians(this double angle)
		{
			return (angle % 360) * (Math.PI / 180);
		}

		public static double RadiansToAngle(this double radian)
		{
			return radian * (180 / Math.PI);
		}

		public static double AngleToRadians(this int angle)
		{
			return (angle % 360) * (Math.PI / 180);
		}

		public static double RadiansToAngle(this int radian)
		{
			return radian * (180 / Math.PI);
		}

		public static Color[] ConvertMediaColorToDrawingColor(this System.Windows.Media.Color[] colors)
		{
			Color[] drawingColors = new Color[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				drawingColors[i] = colors[i].ConvertMediaColorToDrawingColor();
			}
			return drawingColors;
		}

		public static Color ConvertMediaColorToDrawingColor(this System.Windows.Media.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static System.Windows.Media.Color[] ConvertDrawingColorToMediaColor(this Color[] colors)
		{
			System.Windows.Media.Color[] mediaColors = new System.Windows.Media.Color[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				mediaColors[i] = colors[i].ConvertDrawingColorToMediaColor();
			}
			return mediaColors;
		}

		public static System.Windows.Media.Color ConvertDrawingColorToMediaColor(this Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static Point[] ConvertWindowPointToDrawingPoint(this System.Windows.Point[] points)
		{
			Point[] drawingPoints = new Point[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				drawingPoints[i] = points[i].ConvertWindowPointToDrawingPoint();
			}
			return drawingPoints;
		}

		public static Point ConvertWindowPointToDrawingPoint(this System.Windows.Point point)
		{
			return new Point((int)point.X, (int)point.Y);
		}

		public static BitmapSource ConvertBitmap(this Bitmap source)
		{
			return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						  source.GetHbitmap(),
						  IntPtr.Zero,
						  System.Windows.Int32Rect.Empty,
						  BitmapSizeOptions.FromEmptyOptions());
		}

		public static Bitmap BitmapFromSource(this BitmapSource bitmapsource)
		{
			Bitmap bitmap;
			using (var outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapsource));
				enc.Save(outStream);
				bitmap = new Bitmap(outStream);
			}
			return bitmap;
		}

		public static void Clamp(ref this int value, int min, int max)
		{
			if (value > max)
			{
				value = max;
			}
			else if (value < min)
			{
				value = min;
			}
		}

		public static void Clamp(ref this double value, double min, double max)
		{
			if (value > max)
			{
				value = max;
			}
			else if (value < min)
			{
				value = min;
			}
		}
	}
}
