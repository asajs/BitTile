using NUnit.Framework;
using System.Drawing;

namespace BitTileTests.UserControls.ColorPicker
{
	public class ColorHelperTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		[TestCase(Color.Black, 0.0, 0.0, 0.0, 0.0)]
		public void ColorHelper_ConvertRGBAtoHSLA(Color rgba, double hue, double sat, double lum, double alpha)
		{
			Assert.Pass();
		}
	}
}
