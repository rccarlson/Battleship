using static Battleship.Utility;
namespace Battleship;

internal class Program
{
	static void Main(string[] args)
	{
		var board = new Board(RuleSet.Standard);
		for (int y = 0; y < board.Rules.BoardHeight; y++)
		{
			for (int x = 0; x < board.Rules.BoardWidth; x++)
			{
				board.Shoot(x, y);
				Console.Clear();
				Console.WriteLine(board.ToString());
				Console.WriteLine(board.IsWon ? "Game won" : "");
				Thread.Sleep(200);
			}
		}

	}
}