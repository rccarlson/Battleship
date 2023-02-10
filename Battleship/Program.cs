using System.Diagnostics;
using static Battleship.Utility;
namespace Battleship;

internal class Program
{
	static void Main(string[] args)
	{
		SimulateGame(true);
		for (int i = 0; i < 100; i++) SimulateGame(false).Validate(); // warm caches

		for (int i = 0; i < 100; i++) SimulateGame(false); // hook for profiling

		for (int i = 0; i < 100; i++)
		{
			var average = Enumerable.Range(0, 20_000).AsParallel().Average(_ => ProfileMs(() => SimulateGame(false)));
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

	static Board SimulateGame(bool printProgress)
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
					Thread.Sleep(50);
				}
			}
		}
		return board;
	}
}