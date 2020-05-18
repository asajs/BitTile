using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ExtensionMethods;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

namespace BitTile
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly double _wheelOffsetSelectionCircleToMiddle = 107;
		private readonly double _wheelImageSize = 250;
		private readonly double _beginningOfWheel = 95;
		private readonly Point _colorTip = new Point(110, 55);
		private readonly Point _blackTip = new Point(0, 0);
		private readonly Point _whiteTip = new Point(0, 110);
		
		public MainWindowViewModel()
		{
			_wheelCommand = new DelegateCommand<Image>((image) => MouseClickOnWheel(image), (image) => MouseClickOnWheelCanExecute(image));
			_triangleCommand = new DelegateCommand<Image>((image) => MouseClickOnTriangle(image), (image) => MouseClickOnTriangleCanExecute(image));
			ColorWheelSelectionCircle = new SelectionCircle();
			ColorTriangleSelectionCircle = new SelectionCircle();

			HueSliderValue = 180;
			SaturationSliderValue = 100;
			LuminositySliderValue = 50;
			AlphaSliderValue = 100;

			SetStaticColors();

			ColorWheelImage = ColorWheel.Create();
			Point[] trianglePoints = new Point[] { _colorTip, _blackTip, _whiteTip };
			Color[] triangleColors = new Color[] { HueSelectedColor, LueBlackColor, LueWhiteColor }; 
			ColorTriangleImage = ColorTriangle.Create(trianglePoints, triangleColors.ConvertMediaColorsToDrawingColors(), 110, 110);

		}

		private DelegateCommand<Image> _wheelCommand;
		private DelegateCommand<Image> _triangleCommand;

		private BitmapSource _colorWheelImage;
		private BitmapSource _colorTriangleImage;
		private SelectionCircle _colorWheelSelectionCircle;
		private SelectionCircle _colorTriangleSelectionCircle;

		private Color _leftMouseSelectedColor;
		private Color _hueSelectedColor;
		private Color _satZeroColor;
		private Color _lueBlackColor;
		private Color _lueWhiteColor;

		private int _hueSliderValue;
		private int _saturationSliderValue;
		private int _luminositySliderValue;
		private int _alphaSliderValue;

		#region Colors
		public Color LeftMouseSelectedColor
		{
			get
			{
				return _leftMouseSelectedColor;
			}
			set
			{
				if (value != _leftMouseSelectedColor)
				{
					_leftMouseSelectedColor = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Color HueSelectedColor
		{
			get
			{
				return _hueSelectedColor;
			}
			set
			{
				if (value != _hueSelectedColor)
				{
					_hueSelectedColor = value;
					NotifyPropertyChanged();
					SetStaticColors();
				}
			}
		}

		public Color SatZeroColor
		{
			get
			{
				return _satZeroColor;
			}

			set
			{
				_satZeroColor = value;
				NotifyPropertyChanged();
			}
		}

		public Color LueBlackColor
		{
			get
			{
				return _lueBlackColor;
			}

			set
			{
				_lueBlackColor = value;
				NotifyPropertyChanged();
			}
		}

		public Color LueWhiteColor
		{
			get
			{
				return _lueWhiteColor;
			}

			set
			{
				_lueWhiteColor = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Colors

		#region Slider Values
		public int HueSliderValue
		{
			get
			{
				return _hueSliderValue;
			}
			set
			{
				if (value != _hueSliderValue)
				{
					_hueSliderValue = value;
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					HueSelectedColor = ColorHelper.HslaToRgba(HueSliderValue, 100, 50, 100);
					Point[] trianglePoints = new Point[] { new Point(110, 55), new Point(0, 0), new Point(0, 55), new Point(0, 110) };
					Color[] triangleColors = new Color[] { HueSelectedColor, LueBlackColor, SatZeroColor, LueWhiteColor };
					ColorTriangleImage = ColorTriangle.Create(trianglePoints, triangleColors.ConvertMediaColorsToDrawingColors(), 110, 110);
					ColorWheelSelectionCircle.X = Math.Cos(_hueSliderValue.AngleToRadians()) * _wheelOffsetSelectionCircleToMiddle;
					ColorWheelSelectionCircle.Y = -Math.Sin(_hueSliderValue.AngleToRadians()) * _wheelOffsetSelectionCircleToMiddle;
					NotifyPropertyChanged();
				}
			}
		}

		public int SaturationSliderValue
		{
			get
			{
				return _saturationSliderValue;
			}
			set
			{
				if (value != _saturationSliderValue)
				{
					_saturationSliderValue = value;
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					NotifyPropertyChanged();
				}
			}
		}

		public int LuminositySliderValue
		{
			get
			{
				return _luminositySliderValue;
			}
			set
			{
				if (value != _luminositySliderValue)
				{
					_luminositySliderValue = value;
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					NotifyPropertyChanged();
				}
			}
		}

		public int AlphaSliderValue
		{
			get
			{
				return _alphaSliderValue;
			}
			set
			{
				if (value != _alphaSliderValue)
				{
					_alphaSliderValue = value;
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Slider Values,

		#region Images
		public SelectionCircle ColorWheelSelectionCircle
		{
			get
			{
				return _colorWheelSelectionCircle;
			}
			set
			{
				if(value != _colorWheelSelectionCircle)
				{
					_colorWheelSelectionCircle = value;
					NotifyPropertyChanged();
				}
			}
		}

		public SelectionCircle ColorTriangleSelectionCircle
		{
			get
			{
				return _colorTriangleSelectionCircle;
			}
			set
			{
				if (value != _colorTriangleSelectionCircle)
				{
					_colorTriangleSelectionCircle = value;
					NotifyPropertyChanged();
				}
			}
		}


		public BitmapSource ColorWheelImage
		{
			get
			{
				return _colorWheelImage;
			}
			set
			{
				if (value != _colorWheelImage)
				{
					_colorWheelImage = value;
					NotifyPropertyChanged();
				}
			}
		}

		public BitmapSource ColorTriangleImage
		{
			get
			{
				return _colorTriangleImage;
			}
			set
			{
				if (value != _colorTriangleImage)
				{
					_colorTriangleImage = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion Images

		public DelegateCommand<Image> WheelCommand
		{
			get
			{
				return _wheelCommand;
			}
			set
			{
				if (value != _wheelCommand)
				{
					_wheelCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand<Image> TriangleCommand
		{
			get
			{
				return _triangleCommand;
			}
			set
			{
				if (value != _triangleCommand)
				{
					_triangleCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void SetStaticColors()
		{
			double[] hslaValues = ColorHelper.ExpandDoublesToHSLAValues(ColorHelper.RgbaToHsla(HueSelectedColor));
			SatZeroColor = ColorHelper.HslaToRgba(hslaValues[0], 0, hslaValues[2], hslaValues[3]);
			LueBlackColor = ColorHelper.HslaToRgba(hslaValues[0], hslaValues[1], 0, hslaValues[3]);
			LueWhiteColor = ColorHelper.HslaToRgba(hslaValues[0], hslaValues[1], 100, hslaValues[3]);
		}

		private void MouseClickOnWheel(Image image)
		{
			if (image is IInputElement element)
			{
				System.Windows.Point point = Mouse.GetPosition(element);
				double cartX = point.X - _wheelImageSize / 2;
				double cartY = -point.Y + _wheelImageSize / 2;
				double radians = Math.Atan2(cartX, cartY);
				double angle = radians.RadiansToAngle();
				angle = (-angle + 450) % 360;

				HueSliderValue = (int)angle;
			}
		}

		private bool MouseClickOnWheelCanExecute(Image image)
		{
			bool canExecute = false;
			if(image is IInputElement element)
			{
				System.Windows.Point point = Mouse.GetPosition(element);
				double cartX = point.X - _wheelImageSize / 2;
				double cartY = -point.Y + _wheelImageSize / 2;
				double length = Math.Sqrt(Math.Pow(cartX, 2) + Math.Pow(cartY, 2));
				double measureLength = length - _beginningOfWheel;
				canExecute = 0 < measureLength && measureLength < 25;
			}
			return canExecute;
		}

		private void MouseClickOnTriangle(Image image)
		{
			if (image is IInputElement element)
			{
				System.Windows.Point clickPoint = Mouse.GetPosition(element);
				using (Bitmap img = ColorTriangleImage.BitmapFromSource())
				{
					Color color = img.GetPixel((int)clickPoint.X, (int)clickPoint.Y).ConvertDrawingColorToMediaColor();
					double[] hslaValues = ColorHelper.ExpandDoublesToHSLAValues(ColorHelper.RgbaToHsla(color));
					SaturationSliderValue = (int)hslaValues[1];
					LuminositySliderValue = (int)hslaValues[2];
				}
				double cartX = clickPoint.X - 44;
				double cartY = clickPoint.Y - 55;
				ColorTriangleSelectionCircle.X = cartX;
				ColorTriangleSelectionCircle.Y = cartY;
			}
		}

		private bool MouseClickOnTriangleCanExecute(Image image)
		{
			bool canExecute = false;
			if (image is IInputElement element)
			{
				double areaOfColorTriangle = CalculateArea(_blackTip, _whiteTip, _colorTip);
				double areaOfClickTriangles = 0;
				Point clickPoint = Mouse.GetPosition(element).ConvertWindowPointToDrawingPoint();
				areaOfClickTriangles += CalculateArea(_blackTip, _whiteTip, clickPoint);
				areaOfClickTriangles += CalculateArea(_blackTip, clickPoint, _colorTip);
				areaOfClickTriangles += CalculateArea(clickPoint, _whiteTip, _colorTip);
				canExecute = areaOfColorTriangle - areaOfClickTriangles > -0.1; // -0.1 is the delta. It also allows for "fuzz" in that clicks (very) near the edge work

			}
			return canExecute;
		}

		private double CalculateArea(Point C1, Point C2, Point C3)
		{
			double[,] array =
			{
				{C1.X, C1.Y, 1 },
				{C2.X, C2.Y, 1 },
				{C3.X, C3.Y, 1 }
			};
			Matrix<double> matrix = DenseMatrix.OfArray(array);
			double det = matrix.Determinant();
			return Math.Abs(det) / 2;
		}
	}
}
