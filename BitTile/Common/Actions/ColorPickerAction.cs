using BitTile.Common.Interfaces;

namespace BitTile.Common.Actions
{
	public class ColorPickerAction : IAction
	{
		public void Action(IImageData recievedData)
		{
			GetDataFromImage.GetNormalizedPoints(recievedData.MousePoint,
				recievedData.PixelsWide,
				recievedData.PixelsHigh,
				recievedData.SizeOfPixel,
				out int x,
				out int y);
			recievedData.CurrentColor = recievedData.Colors[x, y];
		}
	}
}
