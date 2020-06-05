using ExtensionMethods;
using MathNet.Numerics.Optimization;
using System.ComponentModel;
using System.Windows;

namespace BitTile
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ColorPickerViewModel _colorPickerViewModel = new ColorPickerViewModel();
		private readonly ColorPicker _colorPicker = new ColorPicker();

		private readonly DrawingSpaceViewModel _drawingSpaceViewModel = new DrawingSpaceViewModel();
		private readonly DrawingSpace _drawingSpace = new DrawingSpace();

		private readonly Options _options = new Options();
		private readonly OptionsViewModel _optionsViewModel = new OptionsViewModel();

		public MainWindow()
		{
			InitializeComponent();
			_colorPicker.DataContext = _colorPickerViewModel;
			RightHandSide.Children.Add(_colorPicker);

			_drawingSpace.DataContext = _drawingSpaceViewModel;
			Middle.Children.Add(_drawingSpace);
			_drawingSpaceViewModel.SetColorOfPen(_colorPickerViewModel.LeftMouseSelectedColor.ConvertMediaColorToDrawingColor());

			_options.DataContext = _optionsViewModel;
			LeftHandSide.Children.Add(_options);

			_colorPickerViewModel.PropertyChanged += ColorPickerPropertyChanged;
			_drawingSpaceViewModel.PropertyChanged += DrawingSpaceViewModelPropertyChanged;
		}

		private void DrawingSpaceViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(_drawingSpaceViewModel.BitTile))
			{
				_optionsViewModel.DrawnImage = _drawingSpaceViewModel.BitTile;
			}
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
