using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public record struct Point(int X, int Y)
	{
		public override string ToString() => $"({X}, {Y})";

		public static explicit operator Point(ReadOnlyPoint p) => new(p.X, p.Y);

		public static bool operator ==(Point a, ReadOnlyPoint b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, ReadOnlyPoint b) => a.X != b.X || a.Y != b.Y;
	}


	public readonly record struct ReadOnlyPoint(int X, int Y)
	{
		public override string ToString() => $"({X}, {Y})";

		public static implicit operator ReadOnlyPoint(Point p) => new(p.X, p.Y);

		public static bool operator ==(ReadOnlyPoint a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(ReadOnlyPoint a, Point b) => a.X != b.X || a.Y != b.Y;

	}
}
