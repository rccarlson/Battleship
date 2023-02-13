using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public enum Orientation { Horizontal, Vertical }
	/// <summary>
	/// Describes a set of points that a <see cref="Ship"/> occupies.
	/// A set of these points is created during the first game, and never again.
	/// </summary>
	public struct BoardPlacement
	{
		public BoardPlacement(ReadOnlyPoint start, ReadOnlyPoint end)
		{
			if (start.X != end.X && start.Y != end.Y) throw new ArgumentException($"Cannot place piece diagonally between {start} and {end}");
			Start = start;
			End = end;
			if (start.X == end.X) Orientation = Orientation.Horizontal;
			else if (start.Y == end.Y) Orientation = Orientation.Vertical;
			else throw new ArgumentException("Board placement cannot be diagonal"); // probably unnecessary
		}
		private static T[] RangeAscending<T>(int a, int b, Func<int, T> selector)
		{
			int min, length;
			if (a < b)
				(min, length) = (a, b - a + 1);
			else
				(min, length) = (b, a - b + 1);
			var result = new T[length];
			for (int i = 0; i < length; i++)
				result[i] = selector(i + min);
			return result;
		}

		public Orientation Orientation { get; }
		public int Length => Math.Abs(End.X - Start.X) + Math.Abs(End.Y - Start.Y) + 1;
		private readonly ReadOnlyPoint Start, End;
		private ReadOnlyPoint[]? _occupiedSpaces = null;
		public ReadOnlyPoint[] OccupiedSpaces
		{
			get
			{
				if (_occupiedSpaces is null)
				{
					var start = Start;
					if (Start.X != End.X)
					{
						// horizontal
						_occupiedSpaces = RangeAscending(Start.X, End.X, x => new ReadOnlyPoint(x, start.Y));
					}
					else if (Start.Y != End.Y)
					{
						// vertical
						_occupiedSpaces = RangeAscending(Start.Y, End.Y, y => new ReadOnlyPoint(start.X, y));
					}
					else throw new ArgumentException("Cannot have diagonal piece");
				}
				return _occupiedSpaces ?? Array.Empty<ReadOnlyPoint>();
			}
		}
		public override string ToString() => $"{OccupiedSpaces.First()} -> {OccupiedSpaces.Last()}";
	}

	public readonly record struct Ship(string ShipName, BoardPlacement Placement)
	{
		public int Length => Placement.Length;
	}

}
