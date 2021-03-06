﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using BitTile.Common;
using BitTile.Common.Actions;
using BitTile.UserControls.Prompts;
using ExtensionMethods;
using Microsoft.VisualStudio.PlatformUI;

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

		private NewPrompt _newPrompt = new NewPrompt();

		private bool _saveUpToDate = true;
		private bool _alreadyRanExitCommand = false;

		public DelegateCommand CtrlZCommand { get; set; }
		public DelegateCommand CtrlSCommand { get; set; }
		public DelegateCommand CtrlACommand { get; set; }
		public DelegateCommand CtrlOCommand { get; set; }
		public DelegateCommand CtrlNCommand { get; set; }
		public DelegateCommand ExitCommand { get; set; }

		public MainWindow()
		{
			CtrlZCommand = new DelegateCommand(() => CtrlZ());
			CtrlACommand = new DelegateCommand(() => CtrlA());
			CtrlSCommand = new DelegateCommand(() => CtrlS());
			CtrlOCommand = new DelegateCommand(() => CtrlO());
			CtrlNCommand = new DelegateCommand(() => CtrlN());
			ExitCommand = new DelegateCommand(() => Exit());

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
			_optionsViewModel.ActionChanged += OptionsActionChanged;

			_optionsViewModel.DrawnImage = _drawingSpaceViewModel.BitTile;

			_newPrompt.CreateNewEvent += CreateNew;
			_newPrompt.CancelCreateNewEvent += CloseCreateNewPrompt;
		}

		private void CtrlZ()
		{
			_drawingSpaceViewModel.Undo();
		}

		private void CtrlA()
		{
			_saveUpToDate = _fileHandler.SaveAs(_drawingSpaceViewModel.BitTile);
		}

		private void CtrlN()
		{
			_newPrompt.ShowDialog();
			CloseCreateNewPrompt(null, null);
		}

		private void CreateNew(object sender, CreateNewEventArgs e)
		{
			CloseCreateNewPrompt(sender, null);
			_drawingSpaceViewModel.NewSheet(e.PixelHeight, e.PixelWidth, e.SizeOfPixel);
			_optionsViewModel.HeightOfImage = e.PixelHeight;
			_optionsViewModel.WidthOfImage = e.PixelWidth;
		}

		private void CloseCreateNewPrompt(object sender, EventArgs e)
		{
			if (_newPrompt != null)
			{
				_newPrompt.CreateNewEvent -= CreateNew;
				_newPrompt.CancelCreateNewEvent -= CloseCreateNewPrompt;
				_newPrompt.Close();
			}
			_newPrompt = new NewPrompt();
			_newPrompt.CreateNewEvent += CreateNew;
			_newPrompt.CancelCreateNewEvent += CloseCreateNewPrompt;
		}

		private void CtrlO()
		{
			BitmapSource source = _fileHandler.Open();
			if (source != null)
			{
				_drawingSpaceViewModel.HandleSource(source);
			}
			_saveUpToDate = true;
		}

		private void CtrlS()
		{
			_saveUpToDate = _fileHandler.Save(_drawingSpaceViewModel.BitTile);
		}

		private void Exit()
		{
			if (!_saveUpToDate)
			{
				MessageBoxResult result = MessageBox.Show("Do you want to save this image?", "Save image", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					_fileHandler.SaveAs(_drawingSpaceViewModel.BitTile);
				}
			}
			_alreadyRanExitCommand = true;
			Application.Current.Shutdown();
		}

		private void DrawingSpaceViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_drawingSpaceViewModel.BitTile):
					_optionsViewModel.DrawnImage = _drawingSpaceViewModel.BitTile;
					_saveUpToDate = false;
					break;
				case nameof(_drawingSpaceViewModel.CurrentColor):
					_colorPickerViewModel.SetColor(_drawingSpaceViewModel.CurrentColor);
					_drawingSpaceViewModel.SetAction(new PencilAction());
					break;
				case nameof(_drawingSpaceViewModel.PixelsHigh):
					_optionsViewModel.HeightOfImage = _drawingSpaceViewModel.PixelsHigh;
					break;
				case nameof(_drawingSpaceViewModel.PixelsWide):
					_optionsViewModel.WidthOfImage = _drawingSpaceViewModel.PixelsWide;
					break;
			}
		}

		private void ColorPickerPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(_colorPickerViewModel.LeftMouseSelectedColor))
			{
				_drawingSpaceViewModel.SetColorOfPen(_colorPickerViewModel.LeftMouseSelectedColor.ConvertMediaColorToDrawingColor());
			}
		}

		private void OptionsActionChanged(object sender, ActionChangedEventArgs e)
		{
			_drawingSpaceViewModel.SetAction(e.Action);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!_alreadyRanExitCommand)
			{
				ExitCommand?.Execute(null);
			}
		}
	}
}
