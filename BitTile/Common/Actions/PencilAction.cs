using BitTile.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace BitTile.Common.Actions
{
	public class PencilAction : IAction
	{
		public DrawingSpaceData Action(DrawingSpaceData recievedData)
		{
			int previousX = recievedData.PreviousX;
			int previousY = recievedData.PreviousY;
			BitmapSource smallBitmap = recievedData.SmallBitmap;
			BitmapSource largeBitmap = recievedData.LargeBitmap;
			Color[,] colors = recievedData.Colors;
			Color currentColor = recievedData.CurrentColor;
			GetDataFromImage.GetNormalizedPoints(recievedData.Element, 
												recievedData.PixelsWide, 
												recievedData.PixelsHigh, 
												recievedData.SizeOfPixel, 
												out int y, 
												out int x);

			if (x != previousX || y != previousY)
			{
				if (previousX == -1)
				{
					previousX = x;
					previousY = y;
				}
				Point[] points = DrawLine(previousX, previousY, x, y);
				foreach (Point savePoint in points)
				{
					colors[(int)savePoint.Y, (int)savePoint.X] = currentColor;
				}
				previousY = y;
				previousX = x;
				smallBitmap = BitmapManipulator.EditTileOfBitmap(smallBitmap, currentColor, points, 1);
				largeBitmap = BitmapManipulator.EditTileOfBitmap(largeBitmap, currentColor, points, recievedData.SizeOfPixel);
			}

			DrawingSpaceData sendData = new DrawingSpaceData(recievedData.Element,
															recievedData.SizeOfPixel,
															recievedData.PixelsHigh,
															recievedData.PixelsWide,
															previousX,
															previousY,
															recievedData.IsLeftMousePressed,
															colors,
															currentColor,
															smallBitmap,
															largeBitmap);
			return sendData;
		}

		private Point[] DrawLine(double x1, double y1, double x2, double y2)
		{
			if (Math.Abs(y2 - y1) < Math.Abs(x2 - x1))
			{
				if (x1 > x2)
				{
					return GrabPointsLow(x2, y2, x1, y1);
				}
				else
				{
					return GrabPointsLow(x1, y1, x2, y2);
				}
			}
			else
			{
				if (y1 > y2)
				{
					return GrabPointsHigh(x2, y2, x1, y1);
				}
				else
				{
					return GrabPointsHigh(x1, y1, x2, y2);
				}
			}
		}

		private static Point[] GrabPointsLow(double x1, double y1, double x2, double y2)
		{
			List<Point> points = new List<Point>();
			double dx = x2 - x1;
			double dy = y2 - y1;

			double yIncrement = 1;

			if (dy < 0)
			{
				yIncrement = -1;
				dy = -dy;
			}

			double D = 2 * dy - dx;
			double y = y1;

			for (int x = (int)x1; x < x2; x++)
			{
				points.Add(new Point(x, y));
				if (D > 0)
				{
					y += yIncrement;
					D -= 2 * dx;
				}
				D += 2 * dy;
			}
			points.Add(new Point(x2, y2));
			return points.ToArray();

		}

		private static Point[] GrabPointsHigh(double x1, double y1, double x2, double y2)
		{
			List<Point> points = new List<Point>();
			double dx = x2 - x1;
			double dy = y2 - y1;

			double xIncrement = 1;

			if (dx < 0)
			{
				xIncrement = -1;
				dx = -dx;
			}

			double D = 2 * dx - dy;
			double x = x1;

			for (int y = (int)y1; y < y2; y++)
			{
				points.Add(new Point(x, y));
				if (D > 0)
				{
					x += xIncrement;
					D -= 2 * dy;
				}
				D += 2 * dx;
			}
			points.Add(new Point(x2, y2));
			return points.ToArray();
		}
	}
}
