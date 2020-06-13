using ExtensionMethods;
using Microsoft.VisualStudio.PlatformUI;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

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

		private readonly FileHandler _fileHandler = new FileHandler();

		public DelegateCommand CtrlZCommand { get; set; }
		public DelegateCommand CtrlSCommand { get; set; }
		public DelegateCommand CtrlACommand { get; set; }
		public DelegateCommand CtrlOCommand { get; set; }
		public DelegateCommand CtrlNCommand { get; set; }


		public MainWindow()
		{
			CtrlZCommand = new DelegateCommand(() => CtrlZ());
			CtrlACommand = new DelegateCommand(() => CtrlA());
			CtrlSCommand = new DelegateCommand(() => CtrlS());
			CtrlOCommand = new DelegateCommand(() => CtrlO());
			CtrlNCommand = new DelegateCommand(() => CtrlN());


			InitializeComponent();
			DataContext = this;

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

		private void CtrlZ()
		{
			_drawingSpaceViewModel.Undo();
		}

		private void CtrlA()
		{
			_fileHandler.SaveAs(_drawingSpaceViewModel.SmallBitTile);
		}

		private void CtrlN()
		{
			_drawingSpaceViewModel.New();
		}

		private void CtrlO()
		{
			BitmapSource source = _fileHandler.Open();
			if(source != null)
			{
				_drawingSpaceViewModel.HandleSource(source);
			}
		}

		private void CtrlS()
		{
			_fileHandler.Save(_drawingSpaceViewModel.SmallBitTile);
		}

		private void DrawingSpaceViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(_drawingSpaceViewModel.BitTile))
			{
				_optionsViewModel.DrawnImage = _drawingSpaceViewModel.SmallBitTile;
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
