using BitTile.Common;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.ComponentModel;
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
		private IAction _clickAction;

		private int _previousX = -1;
		private int _previousY = -1;

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
			MouseEnterCommand = new DelegateCommand<Image>((image) => MouseEnter(image));
			MouseLeaveCommand = new DelegateCommand<Image>((image) => MouseLeave(image));
			_undo = new Stack<Color[,]>();
			_clickAction = new PencilAction();

			NewSheet(64, 64, 10);
		}

		#region Commands
		public DelegateCommand<Image> LeftMouseDownCommand { get; set; }

		public DelegateCommand LeftMouseUpCommand { get; set; }

		public DelegateCommand<Image> MouseMoveCommand { get; set; }

		public DelegateCommand<Image> MouseEnterCommand { get; set; }

		public DelegateCommand<Image> MouseLeaveCommand { get; set; }
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

		public Color CurrentColor
		{
			get { return _currentColor; }
			set
			{
				if (value != _currentColor)
				{
					_currentColor = value;
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
			Colors = samples;
			SmallBitTile = BitmapManipulator.CreateBitTile(samples, 1, PixelsHigh, PixelsWide);
			BitTile = BitmapManipulator.CreateBitTile(samples, SizeOfPixel, PixelsHigh, PixelsWide);
			_undo.Clear();
		}

		public void SetColorOfPen(Color color)
		{
			// Intentionally not using the property to avoid triggering a property changed event.
			_currentColor = color; 
		}

		public void SetAction(IAction action)
		{
			_clickAction = action;
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

		private void MouseEnter(Image image)
		{
			_isMouseLeftPressed = Mouse.LeftButton == MouseButtonState.Pressed;
			_previousX = -1;
			_previousY = -1;

			if (_isMouseLeftPressed)
			{
				ChangeBitMap(image);
			}
		}

		private void MouseLeave(Image image)
		{
			if (_isMouseLeftPressed)
			{
				ChangeBitMap(image);
			}
		}

		private void LeftMouseDown(Image image)
		{
			if (image is IInputElement)
			{
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
			_previousX = -1;
			_previousY = -1;
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
				DrawingSpaceData sendData = new DrawingSpaceData(element, SizeOfPixel, PixelsHigh, PixelsWide, _previousX, _previousY,
															_isMouseLeftPressed, Colors, _currentColor, SmallBitTile, BitTile);

				DrawingSpaceData receiveData = _clickAction.Action(sendData);

				SizeOfPixel = receiveData.SizeOfPixel;
				PixelsHigh = receiveData.PixelsHigh;
				PixelsWide = receiveData.PixelsWide;
				_previousX = receiveData.PreviousX;
				_previousY = receiveData.PreviousY;
				Colors = receiveData.Colors;
				CurrentColor = receiveData.CurrentColor;
				SmallBitTile = receiveData.SmallBitmap;
				BitTile = receiveData.LargeBitmap;
			}
		}
		#endregion
	}
}
