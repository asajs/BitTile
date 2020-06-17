using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace BitTile
{
	public static class ColorWheel
	{
		public static BitmapSource Create()
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(210, 210))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					DrawColorWheel(graphics, Color.LightGray, 5, 5, 200, 200);
				}

				image = CreateBitmapSourceFromGdiBitmap(bitmap);
			}
			return image;
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

		// Draw a color wheel in the indicated area.
		private static void DrawColorWheel(Graphics gr, Color outline_color,
			int xmin, int ymin, int wid, int hgt)
		{
			Rectangle rect = new Rectangle(xmin, ymin, wid, hgt);
			GraphicsPath wheel_path = new GraphicsPath();
			wheel_path.AddEllipse(rect);
			wheel_path.Flatten();

			float num_pts = (wheel_path.PointCount - 1) / 6;
			Color[] surround_colors = new Color[wheel_path.PointCount];

			int index = 0;
			InterpolateColors(surround_colors, ref index,
				1 * num_pts, 255, 255, 0, 0, 255, 255, 0, 255);
			InterpolateColors(surround_colors, ref index,
				2 * num_pts, 255, 255, 0, 255, 255, 0, 0, 255);
			InterpolateColors(surround_colors, ref index,
				3 * num_pts, 255, 0, 0, 255, 255, 0, 255, 255);
			InterpolateColors(surround_colors, ref index,
				4 * num_pts, 255, 0, 255, 255, 255, 0, 255, 0);
			InterpolateColors(surround_colors, ref index,
				5 * num_pts, 255, 0, 255, 0, 255, 255, 255, 0);
			InterpolateColors(surround_colors, ref index,
				wheel_path.PointCount, 255, 255, 255, 0, 255, 255, 0, 0);

			using (PathGradientBrush path_brush = new PathGradientBrush(wheel_path))
			{
				path_brush.CenterColor = Color.White;
				path_brush.SurroundColors = surround_colors;
				
				using (Pen pen = new Pen(path_brush, 40))
				{
					gr.DrawPath(pen, wheel_path);

					// It looks better if we outline the wheel.
					//using (Pen thick_pen = new Pen(outline_color, 1))
					//{
					//	gr.DrawPath(thick_pen, wheel_path);
					//}
				}
			}
		}

		// Fill in colors interpolating between the from and to values.
		private static void InterpolateColors(Color[] surround_colors,
			ref int index, float stop_pt,
			int from_a, int from_r, int from_g, int from_b,
			int to_a, int to_r, int to_g, int to_b)
		{
			int num_pts = (int)stop_pt - index;
			float a = from_a, r = from_r, g = from_g, b = from_b;
			float da = (to_a - from_a) / (num_pts - 1);
			float dr = (to_r - from_r) / (num_pts - 1);
			float dg = (to_g - from_g) / (num_pts - 1);
			float db = (to_b - from_b) / (num_pts - 1);

			for (int i = 0; i < num_pts; i++)
			{
				surround_colors[index++] =
					Color.FromArgb((int)a, (int)r, (int)g, (int)b);
				a += da;
				r += dr;
				g += dg;
				b += db;
			}
		}
	}
}
