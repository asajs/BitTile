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
		public NewPrompt()
		{
			PixelHeight = "64";
			PixelWidth = "64";
			SizeOfPixel = "10";
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

			if(widthSuccessful && heightSuccessful && sizeSuccessful)
			{
				width = Math.Max(1, Math.Min(256, width));
				height = Math.Max(1, Math.Min(256, height));
				size = Math.Max(1, Math.Min(20, size));

				CreateNewEventArgs eventArgs = new CreateNewEventArgs(width, height, size);
				CreateNewEvent?.Invoke(this, eventArgs);
			}
			else
			{
				// If they click create and managed to input invalid items, resort to defaults.
				CreateNewEventArgs eventArgs = new CreateNewEventArgs(64, 64, 10);
				CreateNewEvent?.Invoke(this, eventArgs);
			}
		}

		private void ValidateInput(object sender, TextCompositionEventArgs e)
		{
			bool successfulParse = int.TryParse(e.Text, out int result);
			e.Handled = successfulParse;
			if(successfulParse)
			{
				if(sender is TextBox box)
				{
					int index = box.CaretIndex;
					string finalString = box.Text.Insert(box.CaretIndex, result.ToString());
					int.TryParse(finalString, out int finalresult);
					int finalNumber = Math.Max(1, Math.Min(256, finalresult));
					box.Text = finalNumber.ToString();
					box.CaretIndex = index + 1;
				}
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
			bool successfulParse = int.TryParse(e.Text, out int result);
			e.Handled = successfulParse;
			if (successfulParse)
			{
				if (sender is TextBox box)
				{
					int index = box.CaretIndex;
					string finalString = box.Text.Insert(box.CaretIndex, result.ToString());
					int.TryParse(finalString, out int finalresult);
					int finalNumber = Math.Max(1, Math.Min(20, finalresult));
					box.Text = finalNumber.ToString();
					box.CaretIndex = index + 1;
				}
			}
		}
	}
}
