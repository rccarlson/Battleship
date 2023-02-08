namespace BattleshipNeuralNet;
using Battleship;
using static BattleshipNeuralNet.Utility;
using static System.Formats.Asn1.AsnWriter;
using System.Text.Json;

public record Settings//(int TargetPoolSize, int NetworkAttempts, double MutationEpsilon, double MutationRate, double SurvivalRate)
{
	public int TargetPoolSize { get; set; } = 100;
	public int NetworkAttempts { get; set; } = 10; // how many games to average a network's score across
	public double MutationEpsilon { get; set; } = 0.05;
	public double MutationRate { get; set; } = 0.05;
	public double SurvivalRate { get; set; } = 0.50;
	public static Settings Read(string path)
	{
		return JsonSerializer.Deserialize<Settings>(File.ReadAllText(path)) ?? new();
	}
	public void Update(string path)
	{
		var setting = Read(path);
		this.TargetPoolSize = setting.TargetPoolSize;
		this.NetworkAttempts = setting.NetworkAttempts;
		this.MutationEpsilon = setting.MutationEpsilon;
		this.MutationRate = setting.MutationRate;
		this.SurvivalRate = setting.SurvivalRate;
	}
	public void Write(string path)
	{
		var json = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
		File.WriteAllText(path, json);
	}
}

internal class Program
{
	const int TargetPoolSize = 100;
	const int NetworkAttempts = 10; // how many games to average a network's score across
	const double MutationEpsilon = 0.05;
	const double MutationRate = 0.05;
	const double SurvivalRate = 0.50;

	static readonly string SaveFolderPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"Battleship"
			);
	static readonly string SaveFilePath = Path.Combine(
			SaveFolderPath,
			"Networks.dat"
			);

	List<GeneticNeuralNet> Networks = new();
	int Generation = 0;

	static void Main(string[] args)
	{
		//var settings = new Settings(100, 10, 0.05, 0.05, 0.50);
		var settings = new Settings()
		{
			TargetPoolSize = 100,
		};
		settings.Write(Path.Combine(SaveFolderPath, "settings.json"));

		var rules = RuleSet.Standard;
		var state = Load(SaveFilePath);
		// initial setup
		for(int i=state.Networks.Count; i<TargetPoolSize; i++)
		{
			state.Networks.Add(BuildNetwork(rules));
		}
		while (true)
		{
			var scoreboard = state.Networks.AsParallel()
				.Select(net => new NetAndScore(net))
				.OrderBy(net => net.Score)
				.ToArray();
			var scores = scoreboard.Select(sn => sn.Score).ToArray();
			var survivors = scoreboard.Take((int)Math.Round(scoreboard.Length * SurvivalRate)).ToArray();
			state.Networks = survivors.Select(ns => ns.Net).ToList();

			for (int i = state.Networks.Count; i < TargetPoolSize; i++)
			{
				var randomSurvivor = survivors.TakeRandom();
				var clone = randomSurvivor.Net.Clone();
				clone.Mutate(MutationEpsilon, MutationRate);
				state.Networks.Add(clone);
			}

			state.Generation++;
			state.Save(SaveFilePath);
			try
			{
				Console.WriteLine($"{state.Generation}, {scores.Average():F2}, {scores.Min():F5}");
				File.AppendAllLines(
					Path.Combine(SaveFolderPath, "stats.csv"),
					new[] { $"{state.Generation}, {scores.Average():F3}, {scores.Min():F3}" }
					);
			}
			catch { }
		}
	}

	private struct NetAndScore
	{
		public NetAndScore(GeneticNeuralNet net)
		{
			Net = net;
			Score = net.Score(net => SimulateGame(net).ShotsTaken.Count, NetworkAttempts);
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
			for(int y = 0; y < ruleSet.BoardHeight; y++)
			{
				for(int x = 0; x < ruleSet.BoardWidth; x++)
				{
					rowMajorPoints.Add((x, y));
				}
			}
			var inputs = new List<double>(rowMajorPoints.Count * 3);
			foreach(var (x,y) in rowMajorPoints)
			{
				var state = board.GetPointState(x,y);
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
		for(int i = 0; i < networkCount; i++)
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
		using var file = File.OpenWrite(path);
		using var writer = new BinaryWriter(file);
		writer.Write(Generation);
		writer.Write(Networks.Count);
		foreach(var network in Networks)
		{
			GeneticNeuralNet.Write(writer, network);
		}
	}
}