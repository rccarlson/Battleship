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
			var placement = GetAllPossiblePlacements(length, rules, Ships).ToArray().Shuffle().First();
			Ships.Add(new Ship(shipName, placement));
		}
		OccupiedPoints = Ships.SelectMany(ship => ship.Placement.OccupiedSpaces).ToHashSet();
	}

	private static ConcurrentDictionary<string, BoardPlacement[]> BoardPlacementCache = new();
	private static IEnumerable<BoardPlacement> GetAllPossiblePlacements(int length, RuleSet rules, IEnumerable<Ship> ships)
	{
		length -= 1;
		var inputHash = $"{length}|{rules.GetHashCode()}";
		if (!BoardPlacementCache.TryGetValue(inputHash, out BoardPlacement[]? placement))
		{
			var verticalPositions = GeneratePoints(0, rules.BoardWidth, 0, rules.BoardHeight - length)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X, start.Y + length)));
			var horizontalPositions = GeneratePoints(0, rules.BoardWidth - length, 0, rules.BoardHeight)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X + length, start.Y)));
			placement = verticalPositions.Concat(horizontalPositions).ToArray();
			BoardPlacementCache[inputHash] = placement;
		}

		return placement.Where(PlacementIsValid);

		IEnumerable<ReadOnlyPoint> GeneratePoints(int xmin, int xmax, int ymin, int ymax)
			=> Enumerable.Range(xmin, xmax).SelectMany(x => Enumerable.Range(ymin, ymax).Select(y => new ReadOnlyPoint(x, y)));

		bool PlacementIsValid(BoardPlacement placement)
		{
			if (ships is null) return true;
			var allOccupied = ships.SelectMany(ship => ship.Placement.OccupiedSpaces); // spaces currently occupied by a ship
			var allProposed = placement.OccupiedSpaces; // spaces that would be taken if this point is approved
			var conflict = allProposed.Any(proposed => allOccupied.Contains(proposed));
			return !conflict;
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