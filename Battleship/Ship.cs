using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public struct BoardPlacement
	{
		public BoardPlacement(ReadOnlyPoint start, ReadOnlyPoint end)
			{
			if (start.X != end.X && start.Y != end.Y) throw new ArgumentException($"Cannot place piece diagonally between {start} and {end}");
			Start = start;
			End = end;
			}
		private static IEnumerable<int> RangeAscending(int a, int b)
			{
				int min, length;
				if(a < b)
				{
					min = a;
					length = b - a;
				}
				else
				{
					min = b;
					length = a - b;
				}
				return Enumerable.Range(min, length + 1);
			}

		public int Length => Math.Abs(End.X - Start.X) + Math.Abs(End.Y - Start.Y) + 1;
		private ReadOnlyPoint Start, End;
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
						_occupiedSpaces = RangeAscending(Start.X, End.X).Select(x => new ReadOnlyPoint(x, start.Y)).ToArray();
					}
					else if (Start.Y != End.Y)
					{
						// vertical
						_occupiedSpaces = RangeAscending(Start.Y, End.Y).Select(y => new ReadOnlyPoint(start.X, y)).ToArray();
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
