using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	public readonly record struct ReadOnlyPoint(int X, int Y):IEqualityComparer<ReadOnlyPoint>
	{
		public override string ToString() => $"({X}, {Y})";
		public override int GetHashCode() => HashCode.Combine(X, Y);

		public bool Equals(ReadOnlyPoint x, ReadOnlyPoint y) => x.X == y.X && x.Y == y.Y;

		public int GetHashCode([DisallowNull] ReadOnlyPoint obj) => HashCode.Combine(obj.X, obj.Y);
	}
}
