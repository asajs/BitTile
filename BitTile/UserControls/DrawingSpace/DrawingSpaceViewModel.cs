using BitTile.Common;
using ExtensionMethods;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace BitTile
{
	public class DrawingSpaceViewModel : INotifyPropertyChanged
	{
		#region Private Fields
		private Stack<Color[,]> _undo;
		private BitmapSource _bitTile;
		private TileBrush _tileBrush;
		private Color[,] _colors;
		private Color _currentColor;
		private int _height;
		private int _width;
		private int _pixelsHigh;
		private int _pixelsWide;
		private int _sizeOfPixel;
		private bool _isMouseLeftPressed;

		private int _previous_x = -1;
		private int _previous_y = -1;

		private Rect _gridSize;
		private Point _topLeft;
		private Point _topRight;
		private Point _bottomLeft;
		private Point _bottomRight;
		#endregion

		public DrawingSpaceViewModel()
		{
			_isMouseLeftPressed = false;
			LeftMouseDownCommand = new DelegateCommand<Image>((image) => LeftMouseDown(image));
			LeftMouseUpCommand = new DelegateCommand(() => LeftMouseUp());
			MouseMoveCommand = new DelegateCommand<Image>((image) => MouseMove(image), (image) => _isMouseLeftPressed);
			_undo = new Stack<Color[,]>();

			NewSheet(64, 64, 10);
		}

		#region Commands
		public DelegateCommand<Image> LeftMouseDownCommand { get; set; }

		public DelegateCommand LeftMouseUpCommand { get; set; }

		public DelegateCommand<Image> MouseMoveCommand { get; set; }
		#endregion

		#region Properties
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

		public BitmapSource SmallBitTile { get; set; }

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
				if (value != _colors)
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
				if (value != _height)
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
				if (value != _width)
				{
					_width = value;
					NotifyPropertyChanged();
					GridSize = new Rect(0, 0, _width, GridSize.Height);
				}
			}
		}

		public int PixelsHigh
		{
			get { return _pixelsHigh; }
			set
			{
				if (value != _pixelsHigh)
				{
					_pixelsHigh = value;
					_colors = new Color[_pixelsHigh, PixelsWide];
					NotifyPropertyChanged();
					UpdateSecondaryProperties();
				}
			}
		}

		public int PixelsWide
		{
			get { return _pixelsWide; }
			set
			{
				if (value != _pixelsWide)
				{
					_pixelsWide = value;
					_colors = new Color[PixelsHigh, _pixelsWide];
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
				if (value != _sizeOfPixel)
				{
					_sizeOfPixel = value;
					NotifyPropertyChanged();
					TopLeft = new Point(0, 0);
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
		#endregion

		#region Public Methods
		public void Undo()
		{
			if (_undo.Count > 0)
			{
				Colors = _undo.Pop();
				SmallBitTile = BitmapManipulator.CreateBitTile(Colors, 1, PixelsHigh, PixelsWide);
				BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, PixelsHigh, PixelsWide);
			}
		}

		public void NewSheet(int height, int width, int pixelWidth)
		{
			_undo.Clear();

			PixelsHigh = height;
			PixelsWide = width;
			SizeOfPixel = pixelWidth;
			Colors = new Color[PixelsHigh, PixelsWide];
			for (int i = 0; i < PixelsHigh; i++)
			{
				for (int j = 0; j < PixelsWide; j++)
				{
					Colors[i, j] = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
				}
			}
			SmallBitTile = BitmapManipulator.CreateBitTile(Colors, 1, PixelsHigh, PixelsWide);
			BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, PixelsHigh, PixelsWide);
		}

		public void HandleSource(BitmapSource source)
		{
			Color[,] samples = BitmapManipulator.SampleBitmapSource(source, PixelsHigh, PixelsWide);
			SmallBitTile = BitmapManipulator.CreateBitTile(samples, 1, PixelsHigh, PixelsWide);
			BitTile = BitmapManipulator.CreateBitTile(samples, SizeOfPixel, PixelsHigh, PixelsWide);
			_undo.Clear();
		}

		public void SetColorOfPen(Color color)
		{
			_currentColor = color;
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		#region Private Methods
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdateSecondaryProperties()
		{
			Height = PixelsHigh * SizeOfPixel;
			Width = PixelsWide * SizeOfPixel;
			TileBrush = new DrawingBrush
			{
				Viewport = new Rect(0, 0, SizeOfPixel, SizeOfPixel)
			};
		}

		private void LeftMouseDown(Image image)
		{
			if (image is IInputElement element)
			{
				Mouse.Capture(element);
				_isMouseLeftPressed = true;
				Color[,] newColors = new Color[PixelsHigh, PixelsWide];
				for (int i = 0; i < PixelsHigh; i++)
				{
					for (int j = 0; j < PixelsWide; j++)
					{
						newColors[i, j] = Colors[i, j];
					}
				}
				_undo.Push(newColors);
				ChangeBitMap(image);
			}
		}

		private void LeftMouseUp()
		{
			_previous_x = -1;
			_previous_y = -1;
			_isMouseLeftPressed = false;
			Mouse.Capture(null);
		}

		private void MouseMove(Image image)
		{
			ChangeBitMap(image);
		}

		private void ChangeBitMap(Image image)
		{
			if (image is IInputElement element)
			{
				Point point = Mouse.GetPosition(element);
				int y = (int)(point.Y / SizeOfPixel) * SizeOfPixel;
				int x = (int)(point.X / SizeOfPixel) * SizeOfPixel;
				y.Clamp(0, PixelsWide * SizeOfPixel - SizeOfPixel);
				x.Clamp(0, PixelsHigh * SizeOfPixel - SizeOfPixel);
				int colorY = y / SizeOfPixel;
				int colorX = x / SizeOfPixel;
				colorY.Clamp(0, PixelsHigh - 1);
				colorX.Clamp(0, PixelsWide - 1);
				Colors[colorY, colorX] = _currentColor;
				if (colorX != _previous_x || colorY != _previous_y)
				{
					if(_previous_x == -1)
					{
						_previous_x = colorX;
						_previous_y = colorY;
					}
					Point[] points = GrabPoints(_previous_x, _previous_y, colorX, colorY);
					_previous_y = colorY;
					_previous_x = colorX;
					SmallBitTile = BitmapManipulator.EditTileOfBitmap(SmallBitTile, _currentColor, points, 1);
					BitTile = BitmapManipulator.EditTileOfBitmap(BitTile, _currentColor, points, SizeOfPixel);
				}
			}
		}

		public Point[] GrabPoints(double x1, double y1, double x2, double y2)
		{
			if(Math.Abs(y2 - y1) < Math.Abs(x2 - x1))
			{
				if(x1 > x2)
				{
					return GrabPointsLow(x2, y2, x1, y1);
				}
				else
				{
					return GrabPointsLow(x1, y1, x2, y2);
				}
			}
			else
			{
				if (y1 > y2)
				{
					return GrabPointsHigh(x2, y2, x1, y1);
				}
				else
				{
					return GrabPointsHigh(x1, y1, x2, y2);
				}
			}
		}

		private Point[] GrabPointsLow(double x1, double y1, double x2, double y2)
		{
			List<Point> points = new List<Point>();
			double dx = x2 - x1;
			double dy = y2 - y1;

			double yIncrement = 1;

			if(dy < 0)
			{
				yIncrement = -1;
				dy = -dy;
			}

			double D = 2 * dy - dx;
			double y = y1;

			for(int x = (int)x1; x < x2; x++)
			{
				points.Add(new Point(x, y));
				if(D > 0)
				{
					y += yIncrement;
					D -= 2 * dx;
				}
				D += 2 * dy;
			}
			points.Add(new Point(x2, y2));
			return points.ToArray();

		}

		private Point[] GrabPointsHigh(double x1, double y1, double x2, double y2)
		{
			List<Point> points = new List<Point>();
			double dx = x2 - x1;
			double dy = y2 - y1;

			double xIncrement = 1;

			if (dy < 0)
			{
				xIncrement = -1;
				dx = -dx;
			}

			double D = 2 * dx - dy;
			double x = x1;

			for (int y = (int)y1; y < y2; y++)
			{
				points.Add(new Point(x, y));
				if (D > 0)
				{
					x += xIncrement;
					D -= 2 * dy;
				}
				D += 2 * dx;
			}
			points.Add(new Point(x2, y2));
			return points.ToArray();
		}
		#endregion
	}
}
