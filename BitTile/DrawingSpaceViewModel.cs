using ExtensionMethods;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
		private int _numberOfPixelsSize;
		private int _sizeOfPixel;
		private bool _isMouseLeftPressed;

		private int _previous_x = -1;
		private int _previous_y = -1;

		private DelegateCommand<Image> _leftMouseDownCommand;
		private DelegateCommand _leftMouseUpCommand;
		private DelegateCommand<Image> _mouseMoveCommand;

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

			SizeOfPixel = 10;
			NumberOfPixelsSize = 64;
			Colors = new Color[NumberOfPixelsSize, NumberOfPixelsSize];
			for (int i = 0; i < NumberOfPixelsSize; i++)
			{
				for (int j = 0; j < NumberOfPixelsSize; j++)
				{
					Colors[i, j] = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
				}
			}
			SmallBitTile = BitmapManipulator.CreateBitTile(Colors, 1, NumberOfPixelsSize, NumberOfPixelsSize);
			BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
		}

		#region Commands
		public DelegateCommand<Image> LeftMouseDownCommand
		{
			get { return _leftMouseDownCommand; }
			set
			{
				if (value != _leftMouseDownCommand)
				{
					_leftMouseDownCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand LeftMouseUpCommand
		{
			get { return _leftMouseUpCommand; }
			set
			{
				if (value != _leftMouseUpCommand)
				{
					_leftMouseUpCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand<Image> MouseMoveCommand
		{
			get { return _mouseMoveCommand; }
			set
			{
				if (value != _mouseMoveCommand)
				{
					_mouseMoveCommand = value;
					NotifyPropertyChanged();
				}
			}
		}
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

		public int NumberOfPixelsSize
		{
			get { return _numberOfPixelsSize; }
			set
			{
				if (value != _numberOfPixelsSize)
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
				SmallBitTile = BitmapManipulator.CreateBitTile(Colors, 1, NumberOfPixelsSize, NumberOfPixelsSize);
				BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
			}
		}

		public void New()
		{
			for (int i = 0; i < NumberOfPixelsSize; i++)
			{
				for (int j = 0; j < NumberOfPixelsSize; j++)
				{
					Colors[i, j] = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
				}
			}
			SmallBitTile = BitmapManipulator.CreateBitTile(Colors, 1, NumberOfPixelsSize, NumberOfPixelsSize);
			BitTile = BitmapManipulator.CreateBitTile(Colors, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
			_undo.Clear();
		}

		public void HandleSource(BitmapSource source)
		{
			Color[,] samples = BitmapManipulator.SampleBitmapSource(source, NumberOfPixelsSize, NumberOfPixelsSize);
			SmallBitTile = BitmapManipulator.CreateBitTile(samples, 1, NumberOfPixelsSize, NumberOfPixelsSize);
			BitTile = BitmapManipulator.CreateBitTile(samples, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
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
			Height = NumberOfPixelsSize * SizeOfPixel;
			Width = NumberOfPixelsSize * SizeOfPixel;
			TileBrush = new DrawingBrush
			{
				Viewport = new Rect(0, 0, SizeOfPixel, SizeOfPixel)
			};
		}

		private void LeftMouseDown(Image image)
		{
			_isMouseLeftPressed = true;
			Color[,] newColors = new Color[NumberOfPixelsSize, NumberOfPixelsSize];
			for (int i = 0; i < NumberOfPixelsSize; i++)
			{
				for (int j = 0; j < NumberOfPixelsSize; j++)
				{
					newColors[i, j] = Colors[i, j];
				}
			}
			_undo.Push(newColors);
			ChangeBitMap(image);
		}

		private void LeftMouseUp()
		{
			_previous_x = -1;
			_previous_y = -1;
			_isMouseLeftPressed = false;
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
				int x = (int)(point.X / SizeOfPixel) * SizeOfPixel;
				int y = (int)(point.Y / SizeOfPixel) * SizeOfPixel;
				int colorX = x / SizeOfPixel;
				int colorY = y / SizeOfPixel;
				colorX.Clamp(0, NumberOfPixelsSize - 1);
				colorY.Clamp(0, NumberOfPixelsSize - 1);
				Colors[colorX, colorY] = _currentColor;
				if (colorX != _previous_x || colorY != _previous_y)
				{
					_previous_y = colorY;
					_previous_x = colorX;
					SmallBitTile = BitmapManipulator.EditTileOfBitmap(SmallBitTile, _currentColor, colorX, colorY, 1);
					BitTile = BitmapManipulator.EditTileOfBitmap(BitTile, _currentColor, x, y, SizeOfPixel);
				}
			}
		}
		#endregion
	}
}
