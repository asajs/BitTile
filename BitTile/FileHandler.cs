using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace BitTile
{
	public class FileHandler
	{
		private string _pathName;

		public FileHandler()
		{
			_pathName = string.Empty;
		}

		public void Save(BitmapSource source)
		{
			if(string.IsNullOrWhiteSpace(_pathName))
			{
				_pathName = GetPathFromUser();
			}
			if(!string.IsNullOrWhiteSpace(_pathName))
			{
				SaveImage(source);
			}
		}

		public void SaveAs(BitmapSource source)
		{
			_pathName = GetPathFromUser();
			if(!string.IsNullOrWhiteSpace(_pathName))
			{
				SaveImage(source);
			}
		}

		public BitmapSource Open()
		{
			return null;
		}

		private string GetPathFromUser()
		{
			SaveFileDialog dialog = new SaveFileDialog()
			{
				Filter = "PNG (*.png)|*.png"
			};
			if (dialog.ShowDialog() == true)
			{
				return dialog.FileName;
			}
			return _pathName;
		}

		private void SaveImage(BitmapSource source)
		{
			using(FileStream filestream = new FileStream(_pathName, FileMode.Create))
			{
				BitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(filestream);
			}
		}
	}
}
