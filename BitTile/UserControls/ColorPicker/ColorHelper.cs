using System;
using System.Windows.Media;

namespace BitTile
{
	public static class ColorHelper
	{
		/// <summary>
		/// Converts an HSL color value to RGB.
		/// Input: doubles of hue, sat, lue and alpha (alpha defaults to full opqueue )
		/// Output: Color ( A: [0, 255], R: [0, 255], G: [0, 255], B: [0, 255] )
		/// </summary>
		/// <param name="hue">[0, 1.0]</param>
		/// <param name="sat">[0, 1.0]</param>
		/// <param name="lue">[0, 1.0]</param>
		/// <param name="alpha">[0, 1.0]</param>
		/// <returns>ARGB Color</returns>
		public static Color HslaToRgba(double hue, double sat, double lue, double alpha = 100.0)
		{
			hue /= 360.0;
			sat /= 100.0;
			lue /= 100.0;
			alpha /= 100.0;
			double r, g, b;

			if (sat == 0.0)
				r = g = b = lue;

			else
			{
				double q = lue < 0.5 ? lue * (1.0 + sat) : lue + sat - lue * sat;
				double p = 2.0 * lue - q;
				r = HueToRgb(p, q, hue + 1.0 / 3.0);
				g = HueToRgb(p, q, hue);
				b = HueToRgb(p, q, hue - 1.0 / 3.0);
			}
			byte rHex = DoubleToColorByte(r);
			byte gHex = DoubleToColorByte(g);
			byte bHex = DoubleToColorByte(b);
			byte alphaHex = DoubleToColorByte(alpha);

			return Color.FromArgb(alphaHex, rHex, gHex, bHex);
		}

		/// <summary>
		/// Converts an RGB color value to HSL.
		/// Input: Color ( A: [0, 255], R: [0, 255], G: [0, 255], B: [0, 255] )
		/// Output: Array of doubles in format aRGB
		/// </summary>
		/// <param name="rgba"> Color of type System.Windows.Media </param>
		/// <returns>Array of doubles in format RGBA</returns>
		public static double[] RgbaToHsla(Color rgba)
		{
			double red = rgba.R / 255.0;
			double green = rgba.G / 255.0;
			double blue = rgba.B / 255.0;

			double max = (red > green && red > blue) ? red : (green > blue) ? green : blue;
			double min = (red < green && red < blue) ? red : (green < blue) ? green : blue;

			double hue, sat, lue;
			hue = sat = lue = (max + min) / 2.0;

			if (max == min)
				hue = sat = 0.0;

			else
			{
				double d = max - min;
				sat = (lue > 0.5) ? d / (2.0 - max - min) : d / (max + min);

				if (red > green && red > blue)
					hue = (green - blue) / d + (green < blue ? 6.0 : 0.0);

				else if (green > blue)
					hue = (blue - red) / d + 2.0;

				else
					hue = (red - green) / d + 4.0;

				hue /= 6.0;
			}

			return new double[] { hue, sat, lue, rgba.A / 255.0 };
		}

		public static double[] NormalizeHSLAValuesToZeroOne(double hue, double sat, double lue, double alpha)
		{
			hue /= 360.0;
			sat /= 100.0;
			lue /= 100.0;
			alpha /= 100.0;
			return new double[] { hue, sat, lue, alpha };
		}

		public static double[] ExpandDoublesToHSLAValues(double[] values)
		{
			double hue = values[0] * 360.0;
			double sat = values[1] * 100.0;
			double lue = values[2] * 100.0;
			double alpha = values[3] * 100.0;
			return new double[] { hue, sat, lue, alpha };
		}

		// Helper for HslToRgba
		private static double HueToRgb(double p, double q, double t)
		{
			if (t < 0.0) t += 1.0;
			if (t > 1.0) t -= 1.0;
			if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
			if (t < 1.0 / 2.0) return q;
			if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
			return p;
		}

		private static byte DoubleToColorByte(double value)
		{
			return Convert.ToByte((int)(value * 255));
		}

	}
}
