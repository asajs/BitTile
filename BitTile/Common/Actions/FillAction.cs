using BitTile.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Windows.Point;

namespace BitTile.Common.Actions
{
	public class FillAction : IAction
	{
		public void Action(IImageData recievedData)
		{
			Color[,] colors = new Color[recievedData.PixelsHigh, recievedData.PixelsWide];
			Array.Copy(recievedData.Colors, colors, recievedData.PixelsHigh * recievedData.PixelsWide);
			Color currentColor = recievedData.CurrentColor;

			GetDataFromImage.GetNormalizedPoints(recievedData.MousePoint,
									recievedData.PixelsWide,
									recievedData.PixelsHigh,
									recievedData.SizeOfPixel,
									out int y,
									out int x);

			if (colors[y, x] != currentColor)
			{
				IEnumerable<Point> pointsToFill = PointsGrabber.GrabPointsWithinFuzzValue(colors, x, y);

				foreach (Point point in pointsToFill)
				{
					colors[(int)point.Y, (int)point.X] = currentColor;
				}

				recievedData.Colors = colors;
			}
		}
	}
}
