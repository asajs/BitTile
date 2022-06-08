using ExtensionMethods;
using System.Windows;

namespace BitTile.Common
{
	public static class GetDataFromImage
	{
		public static void GetNormalizedPoints(Point mousePosition, int pixelsWide, int pixelsHigh, int sizeOfPixel, out int colorY, out int colorX)
		{
			int y = (int)(mousePosition.Y / sizeOfPixel) * sizeOfPixel;
			int x = (int)(mousePosition.X / sizeOfPixel) * sizeOfPixel;
			y.Clamp(0, pixelsHigh * sizeOfPixel - sizeOfPixel);
			x.Clamp(0, pixelsWide * sizeOfPixel - sizeOfPixel);
			colorY = y / sizeOfPixel;
			colorX = x / sizeOfPixel;
			colorY.Clamp(0, pixelsHigh - 1);
			colorX.Clamp(0, pixelsWide - 1);
		}

		//public static Color GetColorFromPoint(IInputElement element, BitmapSource source, out Color color)
		//{
		//	color = BitmapManipulator.SampleSingleRegion(source, (int)point.Y, (int)point.X);
		//}
	}
}
