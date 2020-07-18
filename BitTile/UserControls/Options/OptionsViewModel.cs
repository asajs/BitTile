using BitTile.Common;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BitTile
{
	public class OptionsViewModel : INotifyPropertyChanged
	{
		private BitmapSource _drawnImage;
		private BitmapSource _pencilImage;
		private BitmapSource _colorPickerImage;
		private BitmapSource _fillImage;
		private double _widthOfImage;
		private double _heightOfImage;
		private double _widthOf3Images;
		private double _heightOf3Images;
		private Rect _sizeOfImage;

		public OptionsViewModel()
		{
			LeftMouseDownOnPencilCommand = new DelegateCommand(() => NotifyActionChanged("pencil"));
			LeftMouseDownOnColorPickerCommand = new DelegateCommand(() => NotifyActionChanged("colorpicker"));
			LeftMouseDownOnFillCommand = new DelegateCommand(() => NotifyActionChanged("fill"));

			WidthOfImage = 64;
			HeightOfImage = 64;

			Assembly pencilAssembly = Assembly.GetExecutingAssembly();
			Stream pencilStream = pencilAssembly.GetManifestResourceStream("BitTile.Resources.pencil.png");
			PencilImage = BitmapManipulator.CreateBitmapSourceFromGdiBitmap(new Bitmap(pencilStream));

			Assembly colorPickerAssembly = Assembly.GetExecutingAssembly();
			Stream colorPickerStream = colorPickerAssembly.GetManifestResourceStream("BitTile.Resources.colorpicker.png");
			ColorPickerImage = BitmapManipulator.CreateBitmapSourceFromGdiBitmap(new Bitmap(colorPickerStream));

			Assembly fillAssembly = Assembly.GetExecutingAssembly();
			Stream fillStream = fillAssembly.GetManifestResourceStream("BitTile.Resources.fill.png");
			FillImage = BitmapManipulator.CreateBitmapSourceFromGdiBitmap(new Bitmap(fillStream));
		}

		public DelegateCommand LeftMouseDownOnPencilCommand { get; set; }
		public DelegateCommand LeftMouseDownOnColorPickerCommand { get; set; }
		public DelegateCommand LeftMouseDownOnFillCommand { get; set; }


		public BitmapSource DrawnImage
		{
			get { return _drawnImage; }
			set
			{
				if(_drawnImage != value)
				{
					_drawnImage = value;
					NotifyPropertyChanged();
				}
			}
		}

		public BitmapSource PencilImage
		{
			get { return _pencilImage; }
			set
			{
				if (_pencilImage != value)
				{
					_pencilImage = value;
					NotifyPropertyChanged();
				}

			}
		}

		public BitmapSource ColorPickerImage
		{
			get { return _colorPickerImage; }
			set
			{
				if (_colorPickerImage != value)
				{
					_colorPickerImage = value;
					NotifyPropertyChanged();
				}

			}
		}

		public BitmapSource FillImage
		{
			get { return _fillImage; }
			set
			{
				if (_fillImage != value)
				{
					_fillImage = value;
					NotifyPropertyChanged();
				}

			}
		}

		public double WidthOfImage
		{
			get { return _widthOfImage; }
			set
			{
				if(_widthOfImage != value)
				{
					_widthOfImage = value;
					WidthOf3Images = value * 3;
					SizeOfImage = new Rect(0, 0, value, SizeOfImage.Height);

					NotifyPropertyChanged();
				}
			}
		}

		public double HeightOfImage
		{
			get { return _heightOfImage; }
			set
			{
				if (_heightOfImage != value)
				{
					_heightOfImage = value;
					HeightOf3Images = value * 3;
					SizeOfImage = new Rect(0, 0, SizeOfImage.Width, value);
					NotifyPropertyChanged();
				}
			}
		}

		public double HeightOf3Images
		{
			get { return _heightOf3Images; }
			set
			{
				if (_heightOf3Images != value)
				{
					_heightOf3Images = value;
					NotifyPropertyChanged();
				}
			}
		}

		public double WidthOf3Images
		{
			get { return _widthOf3Images; }
			set
			{
				if (_widthOf3Images != value)
				{
					_widthOf3Images = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Rect SizeOfImage
		{
			get { return _sizeOfImage; }
			set
			{
				if(value != _sizeOfImage)
				{
					_sizeOfImage = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<ActionChangedEventArgs> ActionChanged;

		private void NotifyActionChanged([CallerMemberName] string actionName = "pencil")
		{
			ActionChanged?.Invoke(this, new ActionChangedEventArgs(actionName));
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
