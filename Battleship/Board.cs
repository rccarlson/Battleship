using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
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
			Ships = new(rules.Ships.Length);
			var points = GetAllPossiblePlacements(5).ToArray();
			foreach (var (shipName, length) in rules.Ships)
			{
				var placement = GetAllPossiblePlacements(length).ToArray().Shuffle().First();
				Ships.Add(new Ship(shipName, placement));
			}
			OccupiedPoints = Ships.SelectMany(ship => ship.Placement.OccupiedSpaces).ToHashSet();
		}
		private IEnumerable<BoardPlacement> GetAllPossiblePlacements(int length)
		{
			length -= 1;
			var verticalPositions = GeneratePoints(0, Rules.BoardWidth, 0, Rules.BoardHeight - length)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X, start.Y + length)));
			var horizontalPositions = GeneratePoints(0, Rules.BoardWidth - length, 0, Rules.BoardHeight)
				.Select(start => new BoardPlacement(start, new ReadOnlyPoint(start.X + length, start.Y)));

			return verticalPositions.Concat(horizontalPositions)
				.Where(PlacementIsValid);

			IEnumerable<ReadOnlyPoint> GeneratePoints(int xmin, int xmax, int ymin, int ymax)
				=> Enumerable.Range(xmin, xmax).SelectMany(x => Enumerable.Range(ymin, ymax).Select(y => new ReadOnlyPoint(x, y)));

			bool PlacementIsValid(BoardPlacement placement)
			{
				if (Ships is null) return true;
				var allOccupied = Ships.SelectMany(ship => ship.Placement.OccupiedSpaces);
				var allProposed = placement.OccupiedSpaces;
				var conflict = allProposed.Where(proposed => allOccupied.Any(occupied => occupied == proposed)).Any();
				return !conflict;
			}
		}

		public readonly RuleSet Rules;
		public readonly List<Ship> Ships;
		private readonly HashSet<ReadOnlyPoint> OccupiedPoints;
		public readonly List<ReadOnlyPoint> ShotsTaken = new();

		public bool IsWon => !OccupiedPoints.Except(ShotsTaken).Any();

		/// <summary>
		/// <see langword="true"/> if the shot is a hit. <see langword="false"/> otherwise.
		/// </summary>
		public bool Shoot(int x, int y)
		{
			var point = new ReadOnlyPoint(x, y);
			ShotsTaken.Add(point);
			return OccupiedPoints.Contains(point);
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
					sb.Append(outputChar);
				}
				sb.AppendLine("|");
			}
			return sb.ToString();
		}
	}
}
