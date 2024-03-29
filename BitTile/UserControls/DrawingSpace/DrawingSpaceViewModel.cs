﻿using BitTile.Common;
using BitTile.Common.Actions;
using BitTile.Common.Interfaces;
using System;
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
	public class DrawingSpaceViewModel : INotifyPropertyChanged, IImageData
	{
		#region Private Fields
		private readonly int _maxPixelSize;
		private readonly int _minPixelSize;

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
		private Visibility _pixelBoxVisibility;
		private IAction _clickAction;

		private Rect _gridSize;
		private Point _topLeft;
		private Point _topRight;
		private Point _bottomLeft;
		private Point _bottomRight;
		#endregion

		public DrawingSpaceViewModel()
		{
			_maxPixelSize = int.Parse(Properties.Resources.MaxPixelSize);
			_minPixelSize = int.Parse(Properties.Resources.MinPizelSize);

			LeftMouseDownCommand = new DelegateCommand<Image>((image) => LeftMouseDown(image));
			LeftMouseUpCommand = new DelegateCommand(() => LeftMouseUp());
			MouseMoveCommand = new DelegateCommand<Image>((image) => MouseMove(image), (image) => IsMouseLeftPressed);
			MouseEnterCommand = new DelegateCommand<Image>((image) => MouseEnter(image));
			MouseLeaveCommand = new DelegateCommand<Image>((image) => MouseLeave(image));
			MouseWheelCommand = new DelegateCommand((eventArgs) => MouseWheelMove(eventArgs));
			_undo = new Stack<Color[,]>();
			_clickAction = new PencilAction();
			int width = int.Parse(Properties.Resources.DefaultWidth);
			int height = int.Parse(Properties.Resources.DefaultHeight);
			int size = int.Parse(Properties.Resources.DefaultPixelSize);

			NewSheet(height, width, size);
		}

		#region Commands
		public DelegateCommand<Image> LeftMouseDownCommand { get; set; }

		public DelegateCommand LeftMouseUpCommand { get; set; }

		public DelegateCommand<Image> MouseMoveCommand { get; set; }

		public DelegateCommand<Image> MouseEnterCommand { get; set; }

		public DelegateCommand<Image> MouseLeaveCommand { get; set; }

		public DelegateCommand MouseWheelCommand { get; set; }
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

		public Point MousePoint 
		{ 
			get; 
			set; 
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
			set
			{
				BitTile = BitmapManipulator.UpdateBitTile(_colors, value, 1, PixelsHigh, PixelsWide);
				_colors = value;
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
					if (_sizeOfPixel > 3)
					{
						PixelBoxVisibility = Visibility.Visible;
					}
					else
					{
						PixelBoxVisibility = Visibility.Hidden;
					}
				}
			}
		}

		public Visibility PixelBoxVisibility
		{
			get { return _pixelBoxVisibility; }
			set
			{
				if (value != _pixelBoxVisibility)
				{
					_pixelBoxVisibility = value;
					NotifyPropertyChanged();
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

		public int PreviousX
		{
			get;
			set;
		}

		public int PreviousY
		{
			get;
			set;
		}

		public bool IsMouseLeftPressed
		{
			get => Mouse.LeftButton == MouseButtonState.Pressed;
		}

		#endregion

		#region Public Methods
		public void Undo()
		{
			if (_undo.Count > 0)
			{
				Colors = _undo.Pop();
			}
		}

		public void NewSheet(int height, int width, int pixelWidth)
		{
			_undo.Clear();

			PixelsHigh = height;
			PixelsWide = width;
			SizeOfPixel = pixelWidth;
			Color[,] colors = new Color[PixelsHigh, PixelsWide];
			for (int i = 0; i < PixelsHigh; i++)
			{
				for (int j = 0; j < PixelsWide; j++)
				{
					colors[i, j] = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
				}
			}
			Colors = colors;
		}

		public void HandleSource(BitmapSource source)
		{
			Colors = BitmapManipulator.SampleBitmapSource(source, PixelsHigh, PixelsWide);
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

		private void MouseWheelMove(object sender)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control && sender is MouseWheelEventArgs e)
			{
				if (e.Delta > 0 && SizeOfPixel < _maxPixelSize)
				{
					SizeOfPixel++;
				}
				else if (e.Delta < 0 && SizeOfPixel > _minPixelSize)
				{
					SizeOfPixel--;
				}
			}
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
			PreviousX = -1;
			PreviousY = -1;

			if (IsMouseLeftPressed)
			{
				ChangeBitMap(image);
			}
		}

		private void MouseLeave(Image image)
		{
			if (IsMouseLeftPressed)
			{
				ChangeBitMap(image);
			}
		}

		private void LeftMouseDown(Image image)
		{
			if (image is IInputElement)
			{
				Color[,] oldImage = new Color[PixelsHigh, PixelsWide];
				Array.Copy(Colors, oldImage, PixelsHigh * PixelsWide);
				_undo.Push(oldImage);
				ChangeBitMap(image);
			}
		}

		private void LeftMouseUp()
		{
			PreviousX = -1;
			PreviousY = -1;
		}

		private void MouseMove(Image image)
		{
			ChangeBitMap(image);
		}

		private void ChangeBitMap(Image image)
		{
			if (image is IInputElement element)
			{
				MousePoint = Mouse.GetPosition(element);
				
				_clickAction.Action(this);
			}
		}
		#endregion
	}
}
