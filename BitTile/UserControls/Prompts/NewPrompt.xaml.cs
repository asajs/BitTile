using BitTile.Common;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BitTile.UserControls.Prompts
{
	/// <summary>
	/// Interaction logic for NewPrompt.xaml
	/// </summary>
	public partial class NewPrompt : Window
	{
		private readonly int _maxImageWidth;
		private readonly int _minImageWidth;
		private readonly int _maxImageHeight;
		private readonly int _minImageHeight;
		private readonly int _maxPixelSize;
		private readonly int _minPixelSize;
		private readonly int _defaultWidth;
		private readonly int _defaultHeight;
		private readonly int _defaultPixelSize;

		public NewPrompt()
		{
			// Deliberately not fail safe. If it throws an exception you should know right away!
			_maxImageWidth = int.Parse(Properties.Resources.MaxImageWidth);
			_minImageWidth = int.Parse(Properties.Resources.MinImageWidth);
			_maxImageHeight = int.Parse(Properties.Resources.MaxImageHeight);
			_minImageHeight = int.Parse(Properties.Resources.MinImageHeight);
			_maxPixelSize = int.Parse(Properties.Resources.MaxPixelSize);
			_minPixelSize = int.Parse(Properties.Resources.MinPizelSize);
			_defaultWidth = int.Parse(Properties.Resources.DefaultWidth);
			_defaultHeight = int.Parse(Properties.Resources.DefaultHeight);
			_defaultPixelSize = int.Parse(Properties.Resources.DefaultPixelSize);

			PixelHeight = Properties.Resources.DefaultHeight;
			PixelWidth = Properties.Resources.DefaultWidth;
			SizeOfPixel = Properties.Resources.DefaultPixelSize;
			CreateCommand = new DelegateCommand(() => FireCreateEvent());
			CancelClickCommand = new DelegateCommand(() => Cancel());

			InitializeComponent();
		}

		public event EventHandler<CreateNewEventArgs> CreateNewEvent;
		public event EventHandler CancelCreateNewEvent;

		public DelegateCommand CreateCommand { get; set; }

		public DelegateCommand CancelClickCommand { get; set; }

		public string PixelWidth { get; set; }

		public string PixelHeight { get; set; }

		public string SizeOfPixel { get; set; }

		private void FireCreateEvent()
		{
			bool widthSuccessful = int.TryParse(PixelWidth, out int width);
			bool heightSuccessful = int.TryParse(PixelHeight, out int height);
			bool sizeSuccessful = int.TryParse(SizeOfPixel, out int size);

			if (widthSuccessful && heightSuccessful && sizeSuccessful)
			{
				width = Math.Max(_minImageWidth, Math.Min(_maxImageWidth, width));
				height = Math.Max(_minImageHeight, Math.Min(_maxImageHeight, height));
				size = Math.Max(_minPixelSize, Math.Min(_maxPixelSize, size));

				CreateNewEventArgs eventArgs = new CreateNewEventArgs(width, height, size);
				CreateNewEvent?.Invoke(this, eventArgs);
			}
			else
			{
				// If they click create and managed to input invalid items, resort to defaults.
				CreateNewEventArgs eventArgs = new CreateNewEventArgs(_defaultWidth, _defaultHeight, _defaultPixelSize);
				CreateNewEvent?.Invoke(this, eventArgs);
			}
		}

		private void ValidateInput(object sender, TextCompositionEventArgs e)
		{
			if (sender is TextBox box)
			{
				HandleInput(box, e, _minImageWidth, _maxImageHeight);
			}
		}

		private void CancelCommand(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Command == ApplicationCommands.Copy ||
				e.Command == ApplicationCommands.Cut ||
				e.Command == ApplicationCommands.Paste)
			{
				e.Handled = true;
			}
		}

		private void Cancel()
		{
			CancelCreateNewEvent?.Invoke(this, null);
		}

		private void ValidateSizeInput(object sender, TextCompositionEventArgs e)
		{
			if (sender is TextBox box)
			{
				HandleInput(box, e, _minPixelSize, _maxPixelSize);
			}
		}

		private void HandleInput(TextBox box, TextCompositionEventArgs keyStroke, int min, int max)
		{
			bool successfulParse = int.TryParse(keyStroke.Text, out int result);
			keyStroke.Handled = successfulParse;
			if (successfulParse)
			{
				int index = box.CaretIndex;
				string finalString = box.Text.Insert(box.CaretIndex, result.ToString());
				int.TryParse(finalString, out int finalresult);
				int finalNumber = Math.Max(min, Math.Min(max, finalresult));
				box.Text = finalNumber.ToString();
				box.CaretIndex = index + 1;
			}
		}
	}
}
