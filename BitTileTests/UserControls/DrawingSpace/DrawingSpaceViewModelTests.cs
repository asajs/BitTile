using BitTile.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Xunit.Sdk;

namespace BitTile.Tests
{
	[TestClass()]
	public class DrawingSpaceViewModelTests
	{
		[TestMethod()]
		public void GrabPointsTest()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(1, 1, 2, 2);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest2()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(1, 1, 4, 4);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest3()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(1, 1, 4, 12);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest4()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(1, 1, 1, 12);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest5()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(-1, -10, 1, 12);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest6()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(-21, 1, 1, 12);
			Assert.IsTrue(points.Length > 0); 
		}

		[TestMethod()]
		public void GrabPointsTest7()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(-10, -10, -9, -9);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest8()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(11, 11, 0, 0);
			Assert.IsTrue(points.Length > 0); 
		}

		[TestMethod()]
		public void GrabPointsTest9()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = GetDataFromImage.GetNormalizedPoints(31, 10, -21, -50);
			Assert.IsTrue(points.Length > 0); 
		}

		[TestMethod()]
		public void GrabPointsTest10()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(1, 1, 1, 1);
			Assert.IsTrue(points.Length > 0);
		}

		[TestMethod()]
		public void GrabPointsTest11()
		{
			DrawingSpaceViewModel model = new DrawingSpaceViewModel();
			Point[] points = model.GrabPoints(9, 48, 44, 1);
			Assert.IsTrue(points.Length > 0);
		}
	}
}