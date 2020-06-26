using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionMethods;
using Color = System.Drawing.Color;

namespace BitTile.Common
{
	public static class GetDataFromImage
	{
		public static void GetNormalizedPoints(IInputElement element, int pixelsWide, int pixelsHigh, int sizeOfPixel, out int colorY, out int colorX)
		{
			Point point = Mouse.GetPosition(element);
			int y = (int)(point.Y / sizeOfPixel) * sizeOfPixel;
			int x = (int)(point.X / sizeOfPixel) * sizeOfPixel;
			y.Clamp(0, pixelsHigh * sizeOfPixel - sizeOfPixel);
			x.Clamp(0, pixelsWide * sizeOfPixel - sizeOfPixel);
			colorY = y / sizeOfPixel;
			colorX = x / sizeOfPixel;
			colorY.Clamp(0, pixelsHigh - 1);
			colorX.Clamp(0, pixelsWide - 1);
		}

		public static void GetColorFromPoint(IInputElement element, BitmapSource source, out Color color)
		{
			Point point = Mouse.GetPosition(element);
			color = BitmapManipulator.SampleRegion(source, (int)point.Y, (int)point.X);
		}
	}
}
