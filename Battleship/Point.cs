using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public readonly record struct ReadOnlyPoint(int X, int Y)
	{
		public override string ToString() => $"({X}, {Y})";
		public override int GetHashCode() => HashCode.Combine(X, Y);

	}
}
