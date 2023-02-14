using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleship;

public enum PointState
{
	Unknown,
	Miss,
	Hit
}

public class Board
{
	public Board(RuleSet rules)
	{
		Rules = rules;
		Ships = new List<Ship>(rules.Ships.Length);
		foreach (var (shipName, length) in rules.Ships)
		{
			var allPlacements = GetAllPossiblePlacements(length, rules, Ships);
			var placement = allPlacements.ChooseRandom();
			var newShip = new Ship(shipName, placement);
			Ships.Add(newShip);
		}
		OccupiedPoints = Ships.SelectMany(ship => ship.Placement.OccupiedSpaces).ToHashSet();
	}

	private static ConcurrentDictionary<int, BoardPlacement[]> BoardPlacementCache = new();
	internal static BoardPlacement[] GetAllPossiblePlacements(int length, RuleSet rules, IEnumerable<Ship> ships)
	{
		BoardPlacement[]? placement;
		var hash = HashCode.Combine(length, rules);
		if (!BoardPlacementCache.TryGetValue(hash, out placement))
		{
			var maxVert = rules.BoardHeight - length;
			var maxHoriz = rules.BoardWidth - length;
			var verticalPositions = rules.AllBoardPoints.Where(point => point.Y < maxVert)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X, start.Y + length - 1)));
			var horizontalPositions = rules.AllBoardPoints.Where(point => point.X < maxHoriz)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X + length - 1, start.Y)));
			BoardPlacementCache[hash] = placement = verticalPositions.Concat(horizontalPositions).ToArray();
		}

		return placement.Where(PlacementIsValid).ToArray();

		bool PlacementIsValid(BoardPlacement placement)
		{
			if (ships is null) return true;

			foreach (var ship in ships)
			{
				if (ship.Placement.ConflictsWithPlacement(placement)) return false;
			}
			return true;
		}
	}

	public readonly RuleSet Rules;
	public readonly List<Ship> Ships;
	private readonly HashSet<ReadOnlyPoint> OccupiedPoints;
	public readonly List<ReadOnlyPoint> ShotsTaken = new();

	public bool IsWon => !OccupiedPoints.Except(ShotsTaken).Any();

	public void Shoot(int x, int y)
	{
		var point = new ReadOnlyPoint(x, y);
		ShotsTaken.Add(point);
	}

	public PointState GetPointState(int x, int y)
	{
		var point = new ReadOnlyPoint(x, y);
		if (!ShotsTaken.Contains(point)) return PointState.Unknown;
		else if (OccupiedPoints.Contains(point)) return PointState.Hit;
		else return PointState.Miss;
	}

	public void Validate()
	{
		var expectedLengths = Rules.Ships.Select(s => s.Length).OrderBy(s => s).ToArray();
		var actualLengths = Ships.Select(s => s.Length).OrderBy(s => s).ToArray();
		var totalExpectedPoints = expectedLengths.Sum();
		var totalActualPoints = actualLengths.Sum();
		if (totalExpectedPoints != totalActualPoints)
			throw new InvalidOperationException($"Expected ships of size {string.Join(", ", expectedLengths)} " +
				$"but produced ships with sizes {string.Join(", ", actualLengths)}");

		var totalOccupiedSpaces = OccupiedPoints.Count;
		if (totalOccupiedSpaces != totalActualPoints)
			throw new InvalidOperationException($"Mismatch between reported lengths and occupied spaces");

		foreach(var ship in Ships)
		{
			if (ship.Placement.OccupiedSpaces.Any(point => !PointIsValid(point)))
				throw new InvalidOperationException($"A point in placement {ship.Placement} was out of bounds");
		}
		bool PointIsValid(ReadOnlyPoint point)
		{
			if(point.X < 0 || point.Y < 0) return false;
			if(point.X >= Rules.BoardWidth) return false;
			if(point.Y >= Rules.BoardHeight) return false;
			return true;
		}
	}

	public override string ToString() => ToString(false);
	public string ToString(bool reveal)
	{
		StringBuilder sb = new();
		for (int y = 0; y < Rules.BoardHeight; y++)
		{
			for (int x = 0; x < Rules.BoardWidth; x++)
			{
				var state = GetPointState(x, y);
				char outputChar = state switch
				{
					PointState.Unknown => '~',
					PointState.Hit => 'H',
					PointState.Miss => ' ',
					_ => throw new NotImplementedException(),
				};
				var occupied = OccupiedPoints.Contains(new(x, y));
				if (reveal && occupied && state is PointState.Unknown) outputChar = 'O';
				sb.Append(outputChar);
			}
			sb.AppendLine("|");
		}
		return sb.ToString();
	}
}