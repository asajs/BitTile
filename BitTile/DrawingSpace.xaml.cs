using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BitTile
{
	/// <summary>
	/// Interaction logic for DrawingSpace.xaml
	/// </summary>
	public partial class DrawingSpace : UserControl
	{
		public DrawingSpace()
		{
			InitializeComponent();
			//AddPixels();

		}

		private void AddPixels()
		{
			StackPanel vertical = new StackPanel() { Orientation = Orientation.Vertical };
			for (int i = 0; i < 64; i++)
			{
				StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
				for (int j = 0; j < 64; j++)
				{
					Button button = new Button();
					button.Width = 10;
					button.Height = 10;
					button.Background = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
					stackPanel.Children.Add(button);

					//DrawingVisual drawingVisual = new DrawingVisual();
					//DrawingContext context = drawingVisual.RenderOpen();
					//SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
					//Pen pen = new Pen(brush, 2);
					//Rect rect = new Rect(new Size(10, 10));
					//context.DrawRectangle(brush, pen, rect);
					//context.Close();
					//visualCollection.Add(drawingVisual);
				}
				vertical.Children.Add(stackPanel);
			}
				PixelContainer.Children.Add(vertical);
		}


	}
}
