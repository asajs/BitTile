using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace BitTile.Common
{
	public class FillAction : IAction
	{
		public DrawingSpaceData Action(DrawingSpaceData recievedData)
		{
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

			Point[] pointsToFill = PointsGrabber.GrabPointsWithinFuzzValue(colors, x, y);

			foreach(Point point in pointsToFill.ToList())
			{
				colors[(int)point.Y, (int)point.X] = currentColor;
			}

			smallBitmap = BitmapManipulator.EditTileOfBitmap(smallBitmap, currentColor, pointsToFill, 1);
			largeBitmap = BitmapManipulator.EditTileOfBitmap(largeBitmap, currentColor, pointsToFill, recievedData.SizeOfPixel);

			DrawingSpaceData sendData = new DrawingSpaceData(recievedData.Element,
															recievedData.SizeOfPixel,
															recievedData.PixelsHigh,
															recievedData.PixelsWide,
															recievedData.PreviousX,
															recievedData.PreviousY,
															recievedData.IsLeftMousePressed,
															colors,
															currentColor,
															smallBitmap,
															largeBitmap);
			return sendData;
		}
	}
}
