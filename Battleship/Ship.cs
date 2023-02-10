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
		public BoardPlacement(ReadOnlyPoint start, ReadOnlyPoint end) {
			if (start.X != end.X && start.Y != end.Y) throw new ArgumentException($"Cannot place piece diagonally between {start} and {end}");

			if (start.X != end.X)
			{
				// horizontal
				OccupiedSpaces = RangeAscending(start.X, end.X).Select(x => new ReadOnlyPoint(x, start.Y)).ToArray();
			}
			else if (start.Y != end.Y)
			{
				// vertical
				OccupiedSpaces = RangeAscending(start.Y, end.Y).Select(y=> new ReadOnlyPoint(start.X, y)).ToArray();
			}
			else throw new ArgumentException("Cannot have diagonal piece");

			static IEnumerable<int> RangeAscending(int a, int b)
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
		}
		public readonly ReadOnlyPoint[] OccupiedSpaces;
		public override string ToString() => $"{OccupiedSpaces.First()} -> {OccupiedSpaces.Last()}";
	}

	public readonly record struct Ship(string ShipName, BoardPlacement Placement)
	{
		public int Length => Placement.Length;
	}

}
