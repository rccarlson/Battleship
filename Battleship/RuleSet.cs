using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public readonly struct RuleSet
	{
		public RuleSet((string ShipName, int Length)[] Ships, int BoardWidth, int BoardHeight)
		{
			this.Ships = Ships;
			this.BoardWidth = BoardWidth;
			this.BoardHeight = BoardHeight;
			AllBoardCoordinates = new (int, int)[BoardWidth * BoardHeight];
			AllBoardPoints = new ReadOnlyPoint[BoardWidth * BoardHeight];
			for(int y = 0; y < BoardHeight; y++)
			{
				for(int x = 0; x < BoardWidth; x++)
				{
					AllBoardCoordinates[y * BoardWidth + x] = (x, y);
					AllBoardPoints[y * BoardWidth + x] = new(x, y);
				}
			}
		}
		static RuleSet()
		{
			Standard = new(
				Ships: new[]
				{
					("Carrier", 5),
					("Battleship", 4),
					("Destroyer", 3),
					("Submarine", 3),
					("Patrol Boat", 2),
				},
				BoardWidth: 10,
				BoardHeight: 10
				);
		}
		public override int GetHashCode()
		{
			var shipStrings = string.Join("|", Ships.Select(ship => HashCode.Combine(ship.ShipName, ship.Length)));
			return HashCode.Combine(BoardWidth, BoardHeight, shipStrings);
		}

		public static RuleSet Standard { get; }

		public readonly (string ShipName, int Length)[] Ships;
		public readonly int BoardWidth, BoardHeight;
		public readonly (int X, int Y)[] AllBoardCoordinates;
		public readonly ReadOnlyPoint[] AllBoardPoints;
	}
}
