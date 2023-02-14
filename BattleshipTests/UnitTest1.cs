using Battleship;

namespace BattleshipTests;

public class Tests
{
	[SetUp]
	public void Setup()
	{
	}

	[TestCase(1, 1)]
	[TestCase(2, 1)]
	[TestCase(2, 5)]
	[TestCase(-2, 5)]
	[TestCase(-2, 0)]
	public void ReadOnlyPointEquals(int x, int y)
	{
		var point = new ReadOnlyPoint(x, y);
		var point2 = new ReadOnlyPoint(x, y);
		Assert.True(point.Equals(point2));
		Assert.True(point == point2);
		Assert.That(point2, Is.EqualTo(point));
	}

	//       (x, y)(x, y)(x, y)
	[TestCase(0, 0, 0, 5, 0, 3, ExpectedResult = true)]
	[TestCase(0, 0, 0, 5, 0, 5, ExpectedResult = true)]
	[TestCase(0, 0, 0, 5, 0, 0, ExpectedResult = true)]
	[TestCase(0, 0, 0, 5, 0, 6, ExpectedResult = false)]

	[TestCase(1, 0, 2, 0, 0, 0, ExpectedResult = false)]
	[TestCase(1, 0, 2, 0, 0, 1, ExpectedResult = false)]
	[TestCase(1, 0, 2, 0, 0, 2, ExpectedResult = false)]
	[TestCase(1, 0, 2, 0, 0, 3, ExpectedResult = false)]
	[TestCase(1, 0, 2, 0, 0, 3, ExpectedResult = false)]

	[TestCase(1, 1, 2, 1, 0, 3, ExpectedResult = false)] // (1,1) -> (2,1), vert
	[TestCase(1, 1, 2, 1, 1, 3, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 1, 1, ExpectedResult = true)]
	[TestCase(1, 1, 2, 1, 2, 1, ExpectedResult = true)]
	[TestCase(1, 1, 2, 1, 1, 2, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 0, 0, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 1, 0, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 0, 1, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 2, 2, ExpectedResult = false)]
	[TestCase(1, 1, 2, 1, 3, 1, ExpectedResult = false)]
	public bool BoardPlacementContains(int startx, int starty, int endx, int endy, int testx, int testy)
	{
		var start = new ReadOnlyPoint(startx, starty);
		var end = new ReadOnlyPoint(endx, endy);
		var placement = new BoardPlacement(start, end);
		var testPoint = new ReadOnlyPoint(testx, testy);
		return placement.ContainsPoint(testPoint);
	}

	//       (x, y)(x, y)|(x, y)(x, y)
	[TestCase(0, 0, 9, 0, 0, 0, 0, 9, ExpectedResult = true)] // intersection at (0,0)
	[TestCase(1, 0, 9, 0, 0, 0, 0, 9, ExpectedResult = false)]
	[TestCase(0, 0, 9, 0, 1, 0, 9, 0, ExpectedResult = true)] // intersection at (1,0)
	[TestCase(5, 5, 6, 5, 1, 0, 9, 0, ExpectedResult = false)]
	[TestCase(5, 5, 6, 5, 1, 0, 1, 5, ExpectedResult = false)]
	public bool BoardPlacementConflictsWithPlacement(int startx1, int starty1, int endx1, int endy1, int startx2, int starty2, int endx2, int endy2)
	{
		var placement1 = new BoardPlacement((startx1, starty1), (endx1, endy1));
		var placement2 = new BoardPlacement((startx2, starty2), (endx2, endy2));
		return placement1.ConflictsWithPlacement(placement2);
	}

	[TestCase(1)]
	[TestCase(2)]
	[TestCase(3)]
	[TestCase(4)]
	[TestCase(5)]
	public void ExhaustiveBoardPlacementConflictTest(int length)
	{
		BoardPlacement[] allPlacements = Board.GetAllPossiblePlacements(length, RuleSet.Standard, Array.Empty<Ship>());
		foreach (var placement1 in allPlacements)
		{
			foreach (var placement2 in allPlacements)
			{
				var overlap = placement1.OccupiedSpaces.Intersect(placement2.OccupiedSpaces).Any();
				var conflict = placement1.ConflictsWithPlacement(placement2);
				Assert.That(conflict, Is.EqualTo(overlap));
			}
		}
	}

	[Test]
	public void AllPossiblePlacements()
	{
		var rules = RuleSet.Standard;
		List<ReadOnlyPoint> points = new List<ReadOnlyPoint>();
		for (int y = 0; y < rules.BoardHeight; y++)
		{
			for (int x = 0; x < rules.BoardWidth; x++)
			{
				points.Add(new(x, y));
			}
		}
		var rulePoints = rules.AllBoardPoints;

		Assert.That(rulePoints.Except(points), Is.Empty, message: $"{nameof(RuleSet)} contains unexpected board points");
		Assert.That(points.Except(rulePoints), Is.Empty, message: $"{nameof(RuleSet)} is missing expected board points");
	}

	[Test]
	public void ValidateBoards()
	{
		for (int i = 0; i < 10_000; i++)
		{
			var board = new Board(RuleSet.Standard);
			board.Validate();
		}
	}

	[TestCase(0,0,1,0, ExpectedResult = true)]
	[TestCase(0,0,10,0, ExpectedResult = true)]
	[TestCase(0,0,0,1, ExpectedResult = false)]
	[TestCase(0,0,0,10, ExpectedResult = false)]
	[TestCase(1,5,1,10, ExpectedResult = false)]
	[TestCase(1,5,10,5, ExpectedResult = true)]
	public bool BoardPlacementIsHorizontal(int startx, int starty, int endx, int endy)
	{
		var start = new ReadOnlyPoint(startx, starty);
		var end = new ReadOnlyPoint(endx, endy);
		var position = new BoardPlacement(start, end);
		return position.Orientation is Orientation.Horizontal;
	}
}