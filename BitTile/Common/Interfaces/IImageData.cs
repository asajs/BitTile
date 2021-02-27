using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BitTile.Common.Interfaces
{
	public interface IImageData
	{
		public IInputElement MouseElement { get; }
		public int SizeOfPixel { get; }
		public int PixelsHigh { get; }
		public int PixelsWide { get; }
		public int PreviousX { get; }
		public int PreviousY { get; }
		public bool IsMouseLeftPressed { get; }
		public Color[,] Colors { get; set; }
		public Color CurrentColor { get; set; }
		public BitmapSource BitTile { get; }
	}
}
