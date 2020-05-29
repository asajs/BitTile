using ExtensionMethods;
using System.ComponentModel;
using System.Windows;

namespace BitTile
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ColorPickerViewModel _colorPickerViewModel = new ColorPickerViewModel();
		private ColorPicker _colorPicker = new ColorPicker();

		private DrawingSpaceViewModel _drawingSpaceViewModel = new DrawingSpaceViewModel();
		private DrawingSpace _drawingSpace = new DrawingSpace();
		public MainWindow()
		{
			InitializeComponent();
			_colorPicker.DataContext = _colorPickerViewModel;
			RightHandSide.Children.Add(_colorPicker);

			_drawingSpace.DataContext = _drawingSpaceViewModel;
			Middle.Children.Add(_drawingSpace);
			_drawingSpaceViewModel.SetColorOfPen(_colorPickerViewModel.LeftMouseSelectedColor.ConvertMediaColorToDrawingColor());

			_colorPickerViewModel.PropertyChanged += ColorPickerPropertyChanged;
		}

		private void ColorPickerPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(_colorPickerViewModel.LeftMouseSelectedColor))
			{
				_drawingSpaceViewModel.SetColorOfPen(_colorPickerViewModel.LeftMouseSelectedColor.ConvertMediaColorToDrawingColor());
			}
		}
	}
}
