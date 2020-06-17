using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pen = System.Drawing.Pen;

namespace BitTile
{
	public class SelectionCircle : INotifyPropertyChanged
	{
		private double _x;
		private double _y;

		public double X
		{
			get
			{
				return _x;
			}
			set
			{
				if (value != _x)
				{
					_x = value;
					NotifyPropertyChanged();
				}
			}
		}
		
		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				if (value != _y)
				{
					_y = value;
					NotifyPropertyChanged();
				}
			}
		}

		public BitmapSource Image { get; set; }
		public SelectionCircle()
		{
			Image = Create();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private static BitmapSource Create()
		{
			BitmapSource image;
			using (Bitmap bitmap = new Bitmap(10, 10))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					Rectangle rect = new Rectangle(0, 0, 10, 10);
					GraphicsPath wheel_path = new GraphicsPath();
					wheel_path.AddEllipse(rect);
					wheel_path.Flatten();

					using (PathGradientBrush path_brush = new PathGradientBrush(wheel_path))
					{
						using (Pen pen = new Pen(path_brush, 3))
						{
							graphics.DrawPath(pen, wheel_path);
						}
					}
					image = CreateBitmapSourceFromGdiBitmap(bitmap);
				}
				return image;
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
