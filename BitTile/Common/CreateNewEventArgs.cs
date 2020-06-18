using System;

namespace BitTile.Common
{
	public class CreateNewEventArgs : EventArgs
	{
		public CreateNewEventArgs(int pixelWidth, int pixelHeight, int pixelSize)
		{
			PixelWidth = pixelWidth;
			PixelHeight = pixelHeight;
			SizeOfPixel = pixelSize;
		}

		public int PixelWidth { get; set; }

		public int PixelHeight { get; set; }

		public int SizeOfPixel { get; set; }

	}
}
