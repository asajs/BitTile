using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
using System.Windows.Media;

namespace BitTile
{
	public class DrawingSpaceViewModel : INotifyPropertyChanged
	{
		private Color[,] _colors;
		private Color _currentColor;
		private int _height;
		private int _width;
		private int _numberOfPixelsSize = 64;
		private int _sizeOfPixel = 10;

		public DrawingSpaceViewModel()
		{
			_colors = new Color[NumberOfPixelsSize, NumberOfPixelsSize];
			for(int i = 0; i < NumberOfPixelsSize; i++)
			{
				for(int j = 0; j < NumberOfPixelsSize; j++)
				{
					Colors[i, j] = Color.FromArgb(0x00, 0x00, 0x00, 0x00);
				}
			}
		}

		public Color[,] Colors
		{
			get { return _colors; }
			private set
			{
				if(value != _colors)
				{
					_colors = value;
					NotifyPropertyChanged();
				}
			}
		}


		public int Height
		{
			get { return _height; }
			private set
			{
				if(value != _height)
				{
					_height = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int Width
		{
			get { return _width; }
			private set
			{
				if(value != _width)
				{
					_width = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int NumberOfPixelsSize
		{
			get { return _numberOfPixelsSize; }
			set
			{
				if(value != _numberOfPixelsSize)
				{
					_numberOfPixelsSize = value;
					_colors = new Color[_numberOfPixelsSize, _numberOfPixelsSize];
					Width = _numberOfPixelsSize * SizeOfPixel;
					Height = _numberOfPixelsSize * SizeOfPixel;
					NotifyPropertyChanged();
				}
			}
		}

		public int SizeOfPixel
		{
			get { return _sizeOfPixel; }
			set
			{
				if(value != _sizeOfPixel)
				{
					_sizeOfPixel = value;
					NotifyPropertyChanged();
				}
			}
		}

		public void SetColorOfPen(Color color)
		{
			_currentColor = color;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
