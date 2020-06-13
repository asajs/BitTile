using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
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
				_pathName = GetSavePathFromUser();
			}
			if(!string.IsNullOrWhiteSpace(_pathName))
			{
				SaveImage(source);
			}
		}

		public void SaveAs(BitmapSource source)
		{
			_pathName = GetSavePathFromUser();
			if(!string.IsNullOrWhiteSpace(_pathName))
			{
				SaveImage(source);
			}
		}

		public BitmapSource Open()
		{
			OpenFileDialog open = new OpenFileDialog()
			{
				Filter = "PNG (*.png)|*.png"
			};
			BitmapSource source = null;
			if(open.ShowDialog() == true)
			{
				_pathName = open.FileName;
				source = new BitmapImage(new Uri(_pathName));
			}
			return source;
		}

		private string GetSavePathFromUser()
		{
			SaveFileDialog save = new SaveFileDialog()
			{
				Filter = "PNG (*.png)|*.png"
			};
			if (save.ShowDialog() == true)
			{
				return save.FileName;
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
