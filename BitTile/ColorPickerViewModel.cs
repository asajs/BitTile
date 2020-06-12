using ExtensionMethods;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

namespace BitTile
{
	public class ColorPickerViewModel : INotifyPropertyChanged
	{
		private readonly double _wheelOffsetSelectionCircleToMiddle = 107;
		private readonly double _wheelImageSize = 250;
		private readonly double _beginningOfWheel = 95;
		private const int DIAMOND_SIZE = 150;
		private const double DIAMOND_MULTIPLY_SCALE = (double)DIAMOND_SIZE / 100;
		private const double DIAMOND_DIVIDE_SCALE = 100 / (double)DIAMOND_SIZE;

		public ColorPickerViewModel()
		{
			LeftMouseDownOnWheelCommand = new DelegateCommand<Image>((image) => LeftMouseDownOnWheel(image), (image) => LeftMouseDownOnWheelCanExecute(image));
			LeftMouseUpOnWheelCommand = new DelegateCommand(() => LeftMouseUpOnWheel());
			MouseMoveOnWheelCommand = new DelegateCommand<Image>((image) => MouseMoveOnWheel(image), (image) => _isLeftMouseOnWheelPressed);
			LeftMouseDownOnDiamondCommand = new DelegateCommand<Image>((image) => LeftMouseDownOnDiamond(image), (image) => LeftMouseDownOnDiamondCanExecute(image));
			LeftMouseUpOnDiamondCommand = new DelegateCommand(() => LeftMouseUpOnDiamond());
			MouseMoveOnDiamondCommand = new DelegateCommand<Image>((image) => MouseMoveOnDiamond(image), (image) => _isLeftMouseOnDiamondPressed);

			ColorWheelSelectionCircle = new SelectionCircle();
			ColorDiamondSelectionCircle = new SelectionCircle();

			HueSliderValue = 180;
			SaturationSliderValue = 100;
			LuminositySliderValue = 50;
			AlphaSliderValue = 100;
			SetStaticColors();
			ColorWheelImage = ColorWheel.Create();
			ColorDiamondImage = ColorDiamond.Create(HueSelectedColor, DIAMOND_SIZE);

		}

		private DelegateCommand<Image> _leftMouseDownOnDiamondCommand;
		private DelegateCommand _leftMouseUpOnDiamondCommand;
		private DelegateCommand<Image> _mouseMoveOnDiamondCommand;

		private DelegateCommand<Image> _leftMouseDownOnWheelCommand;
		private DelegateCommand _leftMouseUpOnWheelCommand;
		private DelegateCommand<Image> _mouseMoveOnWheelCommand;

		private BitmapSource _colorWheelImage;
		private BitmapSource _colorDiamondImage;
		private SelectionCircle _colorWheelSelectionCircle;
		private SelectionCircle _colorDiamondSelectionCircle;

		private Color _leftMouseSelectedColor;
		private Color _hueSelectedColor;
		private Color _satZeroColor;
		private Color _lueBlackColor;
		private Color _lueWhiteColor;

		private int _hueSliderValue;
		private int _saturationSliderValue;
		private int _luminositySliderValue;
		private int _alphaSliderValue;

		private bool _isLeftMouseOnWheelPressed;
		private bool _isLeftMouseOnDiamondPressed;


		public int DiamondLengthOfSide
		{
			get { return DIAMOND_SIZE; }
		}


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
					_hueSliderValue.Clamp(0, 360);
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					HueSelectedColor = ColorHelper.HslaToRgba(HueSliderValue, 100, 50, 100);
					ColorDiamondImage = ColorDiamond.Create(HueSelectedColor, DIAMOND_SIZE);
					ColorWheelSelectionCircle.X = Math.Cos(HueSliderValue.AngleToRadians()) * _wheelOffsetSelectionCircleToMiddle;
					ColorWheelSelectionCircle.Y = -Math.Sin(HueSliderValue.AngleToRadians()) * _wheelOffsetSelectionCircleToMiddle;
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
					_saturationSliderValue.Clamp(0, 100);
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					ColorDiamondSelectionCircle.X = CalculateDiamondX(SaturationSliderValue);
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
					_luminositySliderValue.Clamp(0, 100);
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					ColorDiamondSelectionCircle.X = CalculateDiamondX(SaturationSliderValue);
					ColorDiamondSelectionCircle.Y = CalculateDiamondY(LuminositySliderValue);
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
					_alphaSliderValue.Clamp(0, 100);
					Color newColor = ColorHelper.HslaToRgba(HueSliderValue, SaturationSliderValue, LuminositySliderValue, AlphaSliderValue);
					LeftMouseSelectedColor = newColor;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Slider Values

		#region Images
		public SelectionCircle ColorWheelSelectionCircle
		{
			get
			{
				return _colorWheelSelectionCircle;
			}
			set
			{
				if (value != _colorWheelSelectionCircle)
				{
					_colorWheelSelectionCircle = value;
					NotifyPropertyChanged();
				}
			}
		}

		public SelectionCircle ColorDiamondSelectionCircle
		{
			get
			{
				return _colorDiamondSelectionCircle;
			}
			set
			{
				if (value != _colorDiamondSelectionCircle)
				{
					_colorDiamondSelectionCircle = value;
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

		public BitmapSource ColorDiamondImage
		{
			get
			{
				return _colorDiamondImage;
			}
			set
			{
				if (value != _colorDiamondImage)
				{
					_colorDiamondImage = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion Images

		public DelegateCommand<Image> LeftMouseDownOnWheelCommand
		{
			get
			{
				return _leftMouseDownOnWheelCommand;
			}
			set
			{
				if (value != _leftMouseDownOnWheelCommand)
				{
					_leftMouseDownOnWheelCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand LeftMouseUpOnWheelCommand
		{
			get
			{
				return _leftMouseUpOnWheelCommand;
			}
			set
			{
				if (value != _leftMouseUpOnWheelCommand)
				{
					_leftMouseUpOnWheelCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand<Image> MouseMoveOnWheelCommand
		{
			get
			{
				return _mouseMoveOnWheelCommand;
			}
			set
			{
				if (value != _mouseMoveOnWheelCommand)
				{
					_mouseMoveOnWheelCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand<Image> LeftMouseDownOnDiamondCommand
		{
			get
			{
				return _leftMouseDownOnDiamondCommand;
			}
			set
			{
				if (value != _leftMouseDownOnDiamondCommand)
				{
					_leftMouseDownOnDiamondCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand LeftMouseUpOnDiamondCommand
		{
			get
			{
				return _leftMouseUpOnDiamondCommand;
			}
			set
			{
				if (value != _leftMouseUpOnDiamondCommand)
				{
					_leftMouseUpOnDiamondCommand = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DelegateCommand<Image> MouseMoveOnDiamondCommand
		{
			get
			{
				return _mouseMoveOnDiamondCommand;
			}
			set
			{
				if (value != _mouseMoveOnDiamondCommand)
				{
					_mouseMoveOnDiamondCommand = value;
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

		#region Left Mouse Wheel
		private void LeftMouseDownOnWheel(Image image)
		{
			_isLeftMouseOnWheelPressed = true;
			SelectColorOnWheel(image);
		}

		private bool LeftMouseDownOnWheelCanExecute(Image image)
		{
			bool canExecute = false;
			if (image is IInputElement element)
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

		private void MouseMoveOnWheel(Image image)
		{
			SelectColorOnWheel(image);
		}

		private void LeftMouseUpOnWheel()
		{
			_isLeftMouseOnWheelPressed = false;
		}

		private void SelectColorOnWheel(Image image)
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
		#endregion

		#region Left Mouse Diamond
		private void LeftMouseDownOnDiamond(Image image)
		{
			_isLeftMouseOnDiamondPressed = true;
			SelectColorOnDiamond(image);
		}

		private bool LeftMouseDownOnDiamondCanExecute(Image image)
		{
			bool canExecute = false;
			if (image is IInputElement element)
			{
				Point clickPoint = Mouse.GetPosition(element).ConvertWindowPointToDrawingPoint();

				Point bottomTip = new Point(DIAMOND_SIZE / 2, 0);
				Point topTip = new Point(DIAMOND_SIZE / 2, DIAMOND_SIZE);
				Point sideTip = clickPoint.X > DIAMOND_SIZE / 2 ? new Point(DIAMOND_SIZE, DIAMOND_SIZE / 2) : new Point(0, DIAMOND_SIZE / 2); ;

				double areaOfColorDiamond = CalculateArea(bottomTip, topTip, sideTip);
				double areaOfClickDiamonds = 0;
				areaOfClickDiamonds += CalculateArea(bottomTip, topTip, clickPoint);
				areaOfClickDiamonds += CalculateArea(bottomTip, clickPoint, sideTip);
				areaOfClickDiamonds += CalculateArea(clickPoint, topTip, sideTip);
				canExecute = areaOfColorDiamond - areaOfClickDiamonds > -0.1; // -0.1 is the delta. It also allows for "fuzz" in that clicks (very) near the edge work
			}
			return canExecute;
		}

		private void MouseMoveOnDiamond(Image image)
		{
			SelectColorOnDiamond(image);
		}

		private void LeftMouseUpOnDiamond()
		{
			_isLeftMouseOnDiamondPressed = false;
		}

		private void SelectColorOnDiamond(Image image)
		{
			if (image is IInputElement element)
			{
				System.Windows.Point clickPoint = Mouse.GetPosition(element);
				double y = DIAMOND_SIZE * DIAMOND_DIVIDE_SCALE - clickPoint.Y * DIAMOND_DIVIDE_SCALE;
				double distanceFromTopBottom = clickPoint.Y > DIAMOND_SIZE / 2 ? DIAMOND_SIZE - clickPoint.Y : clickPoint.Y;
				double yRatio = distanceFromTopBottom / (DIAMOND_SIZE / 2);
				double xLength = DIAMOND_SIZE * yRatio;
				double modifiedX = clickPoint.X - ((DIAMOND_SIZE - xLength) / 2);
				double sliderValue = modifiedX / xLength * 100;
				SaturationSliderValue = (int)sliderValue;
				LuminositySliderValue = (int)y;
			}
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

		private double CalculateDiamondX(double value)
		{
			double complicated = LuminositySliderValue >= 50 ? 50 - (LuminositySliderValue % 50) : LuminositySliderValue;
			complicated = LuminositySliderValue != 100 ? complicated : 0;
			return (value * DIAMOND_MULTIPLY_SCALE - DIAMOND_SIZE / 2) * complicated / 50;
		}

		private double CalculateDiamondY(double value)
		{
			return DIAMOND_SIZE - (value * DIAMOND_MULTIPLY_SCALE + DIAMOND_SIZE / 2);
		}
		#endregion
	}
}
