using BitTile.Common.Interfaces;
using System.Drawing;

namespace BitTile.Common.Actions
{
	public class ColorPickerAction : IAction
	{
		public DrawingSpaceData Action(DrawingSpaceData recievedData)
		{
			GetDataFromImage.GetColorFromPoint(recievedData.Element, recievedData.LargeBitmap, out Color color);
			DrawingSpaceData sendData = new DrawingSpaceData(recievedData.Element,
															recievedData.SizeOfPixel,
															recievedData.PixelsHigh,
															recievedData.PixelsWide,
															recievedData.PreviousX,
															recievedData.PreviousY,
															recievedData.IsLeftMousePressed,
															recievedData.Colors,
															color,
															recievedData.SmallBitmap,
															recievedData.LargeBitmap);
			return sendData;
		}

	}
}
