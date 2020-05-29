using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

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

		private Rect _gridSize;
		private System.Windows.Point _topLeft;
		private System.Windows.Point _topRight;
		private System.Windows.Point _bottomLeft;
		private System.Windows.Point _bottomRight;

		public DrawingSpaceViewModel()
		{
			SizeOfPixel = 40;
			NumberOfPixelsSize = 16;
			_colors = new Color[NumberOfPixelsSize, NumberOfPixelsSize];
			Random random = new Random();
			for(int i = 0; i < NumberOfPixelsSize; i++)
			{
				for(int j = 0; j < NumberOfPixelsSize; j++)
				{
					//byte alpha = Convert.ToByte(random.Next(0, 255));
					byte alpha = 0xFF;
					byte red = Convert.ToByte(random.Next(0, 255));
					byte green = Convert.ToByte(random.Next(0, 255));
					byte blue = Convert.ToByte(random.Next(0, 255));
					Colors[i, j] = Color.FromArgb(alpha, red, green, blue);
					//Colors[i, j] = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
				}
			}
			BitTile = CreateBitTile(Colors, SizeOfPixel, NumberOfPixelsSize, NumberOfPixelsSize);
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

		public System.Windows.Point TopLeft
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

		public System.Windows.Point TopRight
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

		public System.Windows.Point BottomLeft
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

		public System.Windows.Point BottomRight
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
					TopLeft = new System.Windows.Point(0,0);
					TopRight = new System.Windows.Point(0, _sizeOfPixel);
					BottomRight = new System.Windows.Point(_sizeOfPixel, _sizeOfPixel);
					BottomLeft = new System.Windows.Point(_sizeOfPixel, 0);
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


		private BitmapSource CreateBitTile(Color[,] colors, int pixelSize, int pixelsWide, int pixelsHeight)
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(pixelsWide * pixelSize, pixelsHeight * pixelSize))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					DrawBitMap(graphics, colors, pixelSize, pixelsWide, pixelsHeight);
				}
				image = CreateBitmapSourceFromGdiBitmap(bitmap);
			}
			return image;
		}

		private void DrawBitMap(Graphics gr, Color[,] colors, int pixelSize, int pixelsWide, int pixelsHeight)
		{
			for(int i = 0; i < pixelsHeight; i++)
			{
				for(int j = 0; j < pixelsWide; j++)
				{
					SolidBrush brush = new SolidBrush(colors[i, j]);
					gr.FillRectangle(brush, i * pixelSize, j * pixelSize, pixelSize, pixelSize);
				}
			}
		}

		private static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
		{
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			BitmapData bitmapData = bitmap.LockBits(
				rect,
				ImageLockMode.ReadWrite,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			try
			{
				int size = (rect.Width * rect.Height) * 4;
				return BitmapSource.Create(
					bitmap.Width,
					bitmap.Height,
					bitmap.HorizontalResolution,
					bitmap.VerticalResolution,
					PixelFormats.Bgra32,
					null,
					bitmapData.Scan0,
					size,
					bitmapData.Stride);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}
	}
}
