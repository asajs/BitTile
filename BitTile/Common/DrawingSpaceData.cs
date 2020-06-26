﻿using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BitTile.Common
{
	public struct DrawingSpaceData
	{
		public DrawingSpaceData(IInputElement element, 
								int sizeOfPixel, 
								int pixelsHigh, 
								int pixelsWide,
								int previousX,
								int previousY,
								bool isLeftMousePressed,
								Color[,] colors,
								Color currentColor,
								BitmapSource smallBitmap,
								BitmapSource largeBitmap)

		{
			Element = element;
			SizeOfPixel = sizeOfPixel;
			PixelsHigh = pixelsHigh;
			PixelsWide = pixelsWide;
			PreviousX = previousX;
			PreviousY = previousY;
			IsLeftMousePressed = isLeftMousePressed;
			Colors = colors;
			CurrentColor = currentColor;
			SmallBitmap = smallBitmap;
			LargeBitmap = largeBitmap;
		}

		public IInputElement Element { get; }
		public int SizeOfPixel { get; }
		public int PixelsHigh { get; }
		public int PixelsWide { get; }
		public int PreviousX { get; }
		public int PreviousY { get; }
		public bool IsLeftMousePressed { get; }
		public Color[,] Colors { get; }
		public Color CurrentColor { get; }
		public BitmapSource SmallBitmap { get; }
		public BitmapSource LargeBitmap { get; }
	}
}
