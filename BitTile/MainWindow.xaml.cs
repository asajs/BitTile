using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

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
