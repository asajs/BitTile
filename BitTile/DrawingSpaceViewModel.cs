using Microsoft.VisualStudio.PlatformUI;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;

namespace BitTile
{
	public class DrawingSpaceViewModel : INotifyPropertyChanged
	{
		private BitmapSource _bitTile;
		private TileBrush _tileBrush;
		private Color[,] _colors;
		private Color _currentColor;
		private int _height;
		private int _width;
		private int _numberOfPixelsSize;
		private int _sizeOfPixel;

		private DelegateCommand<Image> _leftMouseCommand;

		private Rect _gridSize;
		private Point _topLeft;
		private Point _topRight;
		private Point _bottomLeft;
		private Point _bottomRight;

		public DrawingSpaceViewModel()
		{
			LeftMouseCommand = new DelegateCommand<Image>((image) => HandleLeftMouseCommand(image));
			SizeOfPixel = 10;
			NumberOfPixelsSize = 64;
			_colors = new Color[NumberOfPixelsSize, NumberOfPixelsSize];
			for(int i = 0; i < NumberOfPixelsSize; i++)
			{
				for(int j = 0; j < NumberOfPixelsSize; j++)
				{
					Colors[i, j] = Color.FromArgb(0xFF, 0xff, 0xff, 0xff);
				}
			}
			BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
		}

		public DelegateCommand<Image> LeftMouseCommand
		{
			get { return _leftMouseCommand; }
			set
			{
				if(value != _leftMouseCommand)
				{
					_leftMouseCommand = value;
					NotifyPropertyChanged();
				}
			}
		}


		public BitmapSource BitTile
		{
			get { return _bitTile; }
			set
			{
				if (value != _bitTile)
				{
					_bitTile = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Point TopLeft
		{
			get { return _topLeft; }
			private set
			{
				if (value != _topLeft)
				{
					_topLeft = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Point TopRight
		{
			get { return _topRight; }
			private set
			{
				if (value != _topRight)
				{
					_topRight = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Point BottomLeft
		{
			get { return _bottomLeft; }
			private set
			{
				if (value != _bottomLeft)
				{
					_bottomLeft = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Point BottomRight
		{
			get { return _bottomRight; }
			private set
			{
				if (value != _bottomRight)
				{
					_bottomRight = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Rect GridSize
		{
			get { return _gridSize; }
			private set
			{
				if (value != _gridSize)
				{
					_gridSize = value;
					NotifyPropertyChanged();
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
					GridSize = new Rect(0, 0, GridSize.Width, _height);
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
					GridSize = new Rect(0, 0, _width, GridSize.Height);
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
					NotifyPropertyChanged();
					UpdateSecondaryProperties();
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
					TopLeft = new Point(0,0);
					TopRight = new Point(0, _sizeOfPixel);
					BottomRight = new Point(_sizeOfPixel, _sizeOfPixel);
					BottomLeft = new Point(_sizeOfPixel, 0);
					UpdateSecondaryProperties();
				}
			}
		}

		public TileBrush TileBrush
		{
			get { return _tileBrush; }
			set
			{
				if (value != _tileBrush)
				{
					_tileBrush = value;
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

		private void UpdateSecondaryProperties()
		{
			Height = NumberOfPixelsSize * SizeOfPixel;
			Width = NumberOfPixelsSize * SizeOfPixel;
			TileBrush = new DrawingBrush
			{
				Viewport = new Rect(0, 0, SizeOfPixel, SizeOfPixel)
			};
		}

		private void HandleLeftMouseCommand(Image image)
		{
			if (image is IInputElement element)
			{
				Point point = Mouse.GetPosition(element);
				int x = (int)(point.X / SizeOfPixel) * SizeOfPixel;
				int y = (int)(point.Y / SizeOfPixel) * SizeOfPixel;
				BitTile = BitmapManipulator.EditTileOfBitmap(BitTile, _currentColor, x, y, SizeOfPixel);
			}
		}
	}
}
