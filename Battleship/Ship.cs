using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship;

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
		if (start.X <= end.X && start.Y <= end.Y)
			(Start, End) = (start, end);
		else if (start.X >= end.X && start.Y >= end.Y)
			(Start, End) = (end, start);
		else
			throw new ArgumentException("idk whats wrong");
		if (start.X == end.X) Orientation = Orientation.Vertical;
		else if (start.Y == end.Y) Orientation = Orientation.Horizontal;
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

	public bool ContainsPoint(ReadOnlyPoint point)
	{
		return Orientation switch
		{
			Orientation.Horizontal => point.Y == Start.Y /* point lies on same row */ && point.X >= Start.X && point.X <= End.X,
			Orientation.Vertical => point.X == Start.X /* point lies on same column */ && point.Y >= Start.Y && point.Y <= End.Y,
			_ => throw new NotImplementedException()
		};
	}
	public bool ConflictsWithPlacement(BoardPlacement placement)
	{
		if (Orientation == Orientation.Horizontal)
		{
			if (placement.Orientation == Orientation.Horizontal) {
				// both horizontal
				if (placement.Start.Y!= Start.Y) return false; // on different rows
				if (IsBetween(placement.Start.X, Start.X, End.X)) return true;
				if (IsBetween(placement.End.X, Start.X, End.X)) return true;
			}
			else
			{
				// this is horiz other is vert
				if (!IsBetween(placement.Start.X, MinX, MaxX)) return false; // other is not on same column
				if (placement.Start.Y == Start.Y) return true;
				if (placement.Start.Y > Start.Y) return false;
				if (placement.End.Y < End.Y) return false;
			}
		}
		else
		{
			if(placement.Orientation == Orientation.Horizontal)
			{
				// this is vert other is horiz
				if (!IsBetween(placement.Start.Y, MinY, MaxY)) return false;
				if (placement.Start.X == Start.X) return true;
				if (placement.Start.X > Start.X) return false;
				if (placement.End.X < End.X) return false;
			}
			else
			{
				// both vert
				if (placement.Start.X != Start.X) return false; // on different columns
				if (IsBetween(placement.Start.Y, Start.Y, End.Y)) return true;
				if (IsBetween(placement.End.Y, Start.Y, End.Y)) return true;
			}
		}

		return placement.OccupiedSpaces.Intersect(OccupiedSpaces).Any();

		static bool IsBetween(int value, int minInclusive, int maxInclusive)
			=> value >= minInclusive && value <= maxInclusive;
	}

	public Orientation Orientation { get; }
	public int Length => Math.Abs(End.X - Start.X) + Math.Abs(End.Y - Start.Y) + 1;
	
	/// <summary> The point with a smaller value </summary>
	private readonly ReadOnlyPoint Start;
	/// <summary> The point with a larger value </summary>
	private readonly ReadOnlyPoint End;

	public int MinX => Start.X;
	public int MaxX => End.X;
	public int MinY => Start.Y;
	public int MaxY => End.Y;

	private ReadOnlyPoint[]? _occupiedSpaces = null;
	public ReadOnlyPoint[] OccupiedSpaces
	{
		get
		{
			if (_occupiedSpaces is null)
			{
				var start = Start;
				if (Start.Y == End.Y)
				{
					// horizontal
					_occupiedSpaces = RangeAscending(Start.X, End.X, x => new ReadOnlyPoint(x, start.Y));
				}
				else if (Start.X == End.X)
				{
					// vertical
					_occupiedSpaces = RangeAscending(Start.Y, End.Y, y => new ReadOnlyPoint(start.X, y));
				}
				else throw new ArgumentException($"Cannot have diagonal piece ({Start} -> {End})");
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
