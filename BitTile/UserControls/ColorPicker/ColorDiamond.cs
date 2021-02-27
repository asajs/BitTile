using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Media.Imaging;
using BitTile.Common;
using ExtensionMethods;
using Color = System.Drawing.Color;

namespace BitTile
{
	public static class ColorDiamond
	{
		public static BitmapSource Create(System.Windows.Media.Color HueColor, int size)
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(size, size))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					Point[] diamondPoints = GetDiamondTipPoints(size);
					List<Color> listColors = GetDiamondTipColors(HueColor);
					DrawColorDiamond(graphics, diamondPoints, listColors.ToArray());
				}

				image = BitmapManipulator.CreateBitmapSourceFromGdiBitmap(bitmap);
			}
			return image;
		}

		private static void DrawColorDiamond(Graphics gr, Point[] points, Color[] colors)
		{
			GraphicsPath DiamondPath = new GraphicsPath();
			DiamondPath.AddPolygon(points);
			DiamondPath.Flatten();

			using (PathGradientBrush pgb = new PathGradientBrush(DiamondPath))
			{
				pgb.CenterColor = medianColor(colors);
				pgb.SurroundColors = colors;
				gr.FillPolygon(pgb, DiamondPath.PathPoints);
			}
		}

		private static Color medianColor(Color[] cols)
		{
			int c = cols.Length;
			return Color.FromArgb(cols.Sum(x => x.A) / c, cols.Sum(x => x.R) / c,
				cols.Sum(x => x.G) / c, cols.Sum(x => x.B) / c);
		}

		private static List<Color> GetDiamondTipColors(System.Windows.Media.Color hue)
		{
			List<Color> colors = new List<Color>() { hue.ConvertMediaColorToDrawingColor() };
			double[] hslaValues = ColorHelper.ExpandDoublesToHSLAValues(ColorHelper.RgbaToHsla(hue));

			// The order these are added matters.
			colors.Add(ColorHelper.HslaToRgba(hslaValues[0], hslaValues[1], 100, hslaValues[3]).ConvertMediaColorToDrawingColor()); // White
			colors.Add(ColorHelper.HslaToRgba(hslaValues[0], 0, hslaValues[2], hslaValues[3]).ConvertMediaColorToDrawingColor()); // Gray
			colors.Add(ColorHelper.HslaToRgba(hslaValues[0], hslaValues[1], 0, hslaValues[3]).ConvertMediaColorToDrawingColor()); // Black
			return colors;
		}

		private static Point[] GetDiamondTipPoints(int size)
		{
			Point colorTip = new Point(size, size / 2);
			Point blackTip = new Point(size / 2, 0);
			Point whiteTip = new Point(size / 2, size);
			Point grayTip = new Point(0, size / 2);
			return new Point[] { colorTip, blackTip, grayTip, whiteTip };
		}
	}
}
