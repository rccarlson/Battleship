using System.Diagnostics;
using static Battleship.Utility;
namespace Battleship;

internal class Program
{
	static void Main(string[] args)
	{
		//SimulateGame(true);
		for (int i = 0; i < 10; i++) SimulateGame(false); // warm caches

		for (int i = 0; i < 1000; i++)
		{
			var average = Enumerable.Range(0, 10_000).AsParallel().Average(_ => ProfileMs(() => SimulateGame(false)));
			Console.WriteLine(average);
		}

	}

	static double ProfileMs(Action action)
	{
		var sw = Stopwatch.StartNew();
		action();
		sw.Stop();
		return sw.Elapsed.TotalMilliseconds;
	}

	static void SimulateGame(bool printProgress)
	{
		var board = new Board(RuleSet.Standard);
		for (int y = 0; y < board.Rules.BoardHeight; y++)
		{
			for (int x = 0; x < board.Rules.BoardWidth; x++)
			{
				board.Shoot(x, y);
				if(printProgress)
				{
					Console.Clear();
					Console.WriteLine(board.ToString());
					Console.WriteLine(board.IsWon ? "Game won" : "");
					Thread.Sleep(100);
				}
			}
		}
	}
}