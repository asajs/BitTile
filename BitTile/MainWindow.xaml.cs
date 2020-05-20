using System.Drawing;
using System.IO;
using System.Windows;

namespace BitTile
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = _mainWindowViewModel;
		}

		public byte[] ImageToByteArray(Bitmap imageIn)
		{
			using (var ms = new MemoryStream())
			{
				imageIn.Save(ms, imageIn.RawFormat);
				return ms.ToArray();
			}
		}
	}
}
