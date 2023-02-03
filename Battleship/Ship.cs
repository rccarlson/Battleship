using System;
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
			Start = start;
			End = end;
			
			// TODO: this is jank. Make it not jank
			var minx = Math.Min(start.X, end.X);
			var maxx = Math.Max(start.X, end.X);
			var miny = Math.Min(start.Y, end.Y);
			var maxy = Math.Max(start.Y, end.Y);
			var length = (maxx - minx) + (maxy - miny); // works iff diagonals not allowed
			var spaces = new List<ReadOnlyPoint>(length);
			for(int x = minx; x <= maxx; x++)
			{
				for(int y = miny; y <= maxy; y++)
				{
					spaces.Add(new ReadOnlyPoint(x, y));
				}
			}
			OccupiedSpaces = spaces.ToArray();
		}
		public readonly ReadOnlyPoint Start, End;
		public readonly ReadOnlyPoint[] OccupiedSpaces;
		public override string ToString() => $"{Start} -> {End}";
	}

	public readonly record struct Ship(string ShipName, BoardPlacement Placement);

}
