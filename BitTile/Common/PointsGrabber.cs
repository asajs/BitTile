using System.Collections.Generic;
using System.Drawing;
using Point = System.Windows.Point;

namespace BitTile.Common
{
	public static class PointsGrabber
	{

		public static IEnumerable<Point> GrabPointsWithinFuzzValue(Color[,] colors, int normalizedX, int normalizedY, int fuzzValue = 0)
		{
			Stack<Point> frontier = new Stack<Point>();
			List<Point> points = new List<Point>();

			frontier.Push(new Point(normalizedX, normalizedY));

			points.Add(new Point(normalizedX, normalizedY));

			while (frontier.Count > 0)
			{
				Point current = frontier.Pop();
				foreach (Point neighbor in GetNeighbors(current, colors, fuzzValue, normalizedX, normalizedY))
				{
					if (!points.Contains(neighbor))
					{
						frontier.Push(neighbor);
						points.Add(neighbor);
					}
				}
			}

			return points;
		}

		private static IEnumerable<Point> GetNeighbors(Point current, Color[,] colors, int fuzzValue, int normalizedX, int normalizedY)
		{
			List<Point> potentialNeighbors = new List<Point>()
			{
				new Point(current.X - 1, current.Y),
				new Point(current.X + 1, current.Y),
				new Point(current.X, current.Y - 1),
				new Point(current.X, current.Y + 1),
			};

			List<Point> neighbors = new List<Point>();

			foreach (Point point in potentialNeighbors)
			{
				if (point.Y < 0 || point.Y >= colors.GetLength(0)
					|| point.X < 0 || point.X >= colors.GetLength(1))
				{
					continue;
				}

				if (ColorWithinFuzzRange(colors[normalizedY, normalizedX], colors[(int)point.Y, (int)point.X], fuzzValue))
				{
					neighbors.Add(point);
				}
			}

			return neighbors;
		}

		private static bool ColorWithinFuzzRange(Color initialColor, Color checkColor, int fuzzValue)
		{
			if(fuzzValue == 0)
			{
				return initialColor == checkColor;
			}
			// TODO: Make this so you can add a fuzz value
			return initialColor == checkColor;
		}
	}
}
