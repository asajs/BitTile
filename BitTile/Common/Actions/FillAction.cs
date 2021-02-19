using BitTile.Common.Interfaces;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace BitTile.Common.Actions
{
	public class FillAction : IAction
	{
		public DrawingSpaceData Action(DrawingSpaceData recievedData)
		{
			BitmapSource smallBitmap = recievedData.SmallBitmap;
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

				smallBitmap = BitmapManipulator.EditTileOfBitmap(smallBitmap, currentColor, pointsToFill, 1);
			}

			DrawingSpaceData sendData = new DrawingSpaceData(recievedData.MouseElement,
															recievedData.SizeOfPixel,
															recievedData.PixelsHigh,
															recievedData.PixelsWide,
															recievedData.PreviousX,
															recievedData.PreviousY,
															recievedData.IsLeftMousePressed,
															colors,
															currentColor,
															smallBitmap);
			return sendData;
		}
	}
}
