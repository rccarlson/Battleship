using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public record RuleSet((string ShipName, int Length)[] Ships, int BoardWidth, int BoardHeight)
	{
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

		public static RuleSet Standard { get; }
	}
}
