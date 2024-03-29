﻿using System.Drawing;
using System.Windows.Media.Imaging;

namespace BitTile.Common.Interfaces
{
	public interface IImageData
	{
		public System.Windows.Point MousePoint { get; }
		public int SizeOfPixel { get; }
		public int PixelsHigh { get; }
		public int PixelsWide { get; }
		public int PreviousX { get; set; }
		public int PreviousY { get; set; }
		public bool IsMouseLeftPressed { get; }
		public Color[,] Colors { get; set; }
		public Color CurrentColor { get; set; }
		public BitmapSource BitTile { get; }
	}
}
