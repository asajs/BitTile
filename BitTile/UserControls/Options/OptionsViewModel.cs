using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BitTile
{
	public class OptionsViewModel : INotifyPropertyChanged
	{
		private BitmapSource _drawnImage;
		private double _widthOfImage;
		private double _heightOfImage;
		private double _widthOf3Images;
		private double _heightOf3Images;
		private Rect _sizeOfImage;

		public OptionsViewModel()
		{
			WidthOfImage = 64;
			HeightOfImage = 64;
		}

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

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
