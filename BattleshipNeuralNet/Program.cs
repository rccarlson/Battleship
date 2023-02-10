namespace BattleshipNeuralNet;
using Battleship;
using System.Diagnostics;

internal class Program
{
	List<GeneticNeuralNet> Networks = new();
	int Generation = 0;

	static void Main(string[] args)
	{
		var rules = RuleSet.Standard;
		var settings = NetSettings.Default;
		var state = Load(settings.NetworkSavePath);
		// initial setup
		state.Networks.Capacity = settings.TargetPoolSize;
		Parallel.For(state.Networks.Count, settings.TargetPoolSize, _ =>
		{
			state.Networks.Add(BuildNetwork(rules));
		});
		while (true)
		{
			var stopwatch = Stopwatch.StartNew();
			settings.Reload();
			var scoreboard = state.Networks.AsParallel()
				.Select(net => new NetAndScore(net))
				.OrderBy(net => net.Score)
				.ToArray();
			var scores = scoreboard.Select(sn => sn.Score).ToArray();
			var survivors = scoreboard.Take((int)Math.Round(scoreboard.Length * settings.SurvivalRate)).ToArray();
			state.Networks = survivors.Select(ns => ns.Net).ToList();

			for (int i = state.Networks.Count; i < settings.TargetPoolSize; i++)
			{
				var randomSurvivor = survivors.TakeRandom();
				var clone = randomSurvivor.Net.Clone();
				clone.Mutate(settings.MutationEpsilon, settings.MutationRate);
				state.Networks.Add(clone);
			}

			state.Generation++;
			state.Save(settings.NetworkSavePath);
			try
			{
				Console.WriteLine($"{state.Generation}, {scores.Average():F2}, {scores.Min():F5}");
				File.AppendAllLines(
					settings.LogCSVPath,
					new[] { $"{state.Generation}, {scores.Average():F3}, {scores.Min():F3}" }
					);
			}
			catch { }
			stopwatch.Stop();
			Console.WriteLine($"Iteration completed in {stopwatch.Elapsed.TotalSeconds:F2} s");
		}
	}

	private readonly struct NetAndScore
	{
		public NetAndScore(GeneticNeuralNet net)
		{
			Net = net;
			Score = net.Score(net => SimulateGame(net).ShotsTaken.Count, NetSettings.Default.NetworkAttempts);
		}
		public readonly GeneticNeuralNet Net;
		public readonly double Score;
	}

	public static GeneticNeuralNet BuildNetwork(RuleSet rules)
	{
		var boardPoints = rules.BoardWidth * rules.BoardHeight;
		var inputs = boardPoints * Enum.GetValues(typeof(PointState)).Length;
		var network = new GeneticNeuralNet(new[] { inputs, LerpLayerSize(0.5), LerpLayerSize(0.75), LerpLayerSize(0.90), boardPoints });
		return network;

		int AsInt(double value) => (int)Math.Ceiling(value);
		int Lerp(int y0, int y1, double x) => AsInt(y0 + x * (y1 - y0));
		int LerpLayerSize(double x) => Lerp(inputs, boardPoints, x);
	}
	static Board SimulateGame(GeneticNeuralNet network)
	{
		var ruleSet = RuleSet.Standard;
		var board = new Board(ruleSet);
		while (!board.IsWon)
		{
			List<(int x, int y)> rowMajorPoints = new(ruleSet.BoardHeight * ruleSet.BoardHeight);
			for (int y = 0; y < ruleSet.BoardHeight; y++)
			{
				for (int x = 0; x < ruleSet.BoardWidth; x++)
				{
					rowMajorPoints.Add((x, y));
				}
			}
			var inputs = new List<double>(rowMajorPoints.Count * 3);
			foreach (var (x, y) in rowMajorPoints)
			{
				var state = board.GetPointState(x, y);
				inputs.Add(state is PointState.Unknown ? 1.0 : 0.0);
				inputs.Add(state is PointState.Hit ? 1.0 : 0.0);
				inputs.Add(state is PointState.Miss ? 1.0 : 0.0);
			}
			var networkInput = inputs.ToArray();
			var networkOutput = network.GetOutputActivation(networkInput);
			var scoreAndPoints = networkOutput.Fold(rowMajorPoints);
			var target = scoreAndPoints
				.OrderByDescending(tuple => tuple.Item1)
				.First(tuple => !board.ShotsTaken.Any(shot => shot.X == tuple.Item2.x && shot.Y == tuple.Item2.y));
			board.Shoot(target.Item2.x, target.Item2.y);
		}
		return board;
	}

	public static Program Load(string path)
	{
		if (!File.Exists(path)) return new();

		using var file = File.OpenRead(path);
		using var reader = new BinaryReader(file);
		var generation = reader.ReadInt32();
		var networkCount = reader.ReadInt32();
		var networks = new GeneticNeuralNet[networkCount];
		for (int i = 0; i < networkCount; i++)
		{
			networks[i] = GeneticNeuralNet.Read(reader);
		}
		return new Program()
		{
			Networks = networks.ToList(),
			Generation = generation,
		};
	}
	public void Save(string path)
	{
		var directory = Path.GetDirectoryName(path);
		if (directory is not null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

		using var file = File.OpenWrite(path);
		using var writer = new BinaryWriter(file);
		writer.Write(Generation);
		writer.Write(Networks.Count);
		foreach (var network in Networks)
		{
			GeneticNeuralNet.Write(writer, network);
		}
	}
}