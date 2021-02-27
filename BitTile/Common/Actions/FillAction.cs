using System.Drawing;
using System.Linq;
using BitTile.Common.Interfaces;
using Point = System.Windows.Point;

namespace BitTile.Common.Actions
{
	public class FillAction : IAction
	{
		public void Action(IImageData recievedData)
		{
			Color[,] colors = recievedData.Colors;
			Color currentColor = recievedData.CurrentColor;

			GetDataFromImage.GetNormalizedPoints(recievedData.MouseElement,
									recievedData.PixelsWide,
									recievedData.PixelsHigh,
									recievedData.SizeOfPixel,
									out int y,
									out int x);

			if (colors[y, x] != currentColor)
			{
				Point[] pointsToFill = PointsGrabber.GrabPointsWithinFuzzValue(colors, x, y);

				foreach (Point point in pointsToFill.ToList())
				{
					colors[(int)point.Y, (int)point.X] = currentColor;
				}

				recievedData.Colors = colors;
			}
		}
	}
}
