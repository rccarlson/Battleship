using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleshipNeuralNet.Utility;

namespace BattleshipNeuralNet;

public class GeneticNeuralNet
{
	public List<Matrix<double>> Weights;
	public int[] LayerDepths;
	public GeneticNeuralNet(List<Matrix<double>> weights, int[] layerDepths)
	{
		Weights = weights;
		LayerDepths = layerDepths;
	}
	public GeneticNeuralNet(int[] layerDepths)
	{
		if (layerDepths.Length < 3) throw new ArgumentException($"Only {layerDepths.Length} layers. Not enough to define an input, output, and hidden layer");
		LayerDepths = layerDepths;
		Weights = new();
		for (int i = 0; i < LayerDepths.Length - 1; i++)
		{
			Weights.Add(Matrix<double>.Build.Random(rows: LayerDepths[i], columns: LayerDepths[i + 1]));
		}
	}

	public double[] GetOutputActivation(params double[] inputActivations)
	{
		if (inputActivations.Length != LayerDepths[0])
			throw new ArgumentException($"Expected {LayerDepths[0]} activation values, but received {inputActivations.Length}");

		var x = Matrix<double>.Build.DenseOfRowArrays(new[] { inputActivations });

		Matrix<double> z,
			a = x;
		for (int i = 0; i < Weights.Count; i++)
		{
			z = a * Weights[i];
			a = Sigmoid(z);
		}

		return a.ToColumnMajorArray();
	}


	internal static double Sigmoid(double z) => 1.0 / (1.0 + Math.Exp(-z));
	internal static double Sigmoid(float z) => 1.0f / (1.0f + (float)Math.Exp(-z));
	internal static Matrix<double> Sigmoid(Matrix<double> matrix)
		=> MatrixPerPoint(matrix, Sigmoid);
	internal static Matrix<T> MatrixPerPoint<T>(Matrix<T> matrix, Func<T, T> func) where T : struct, IEquatable<T>, IFormattable
	{
		var newMatrix = matrix.Clone();
		for (int i = 0; i < matrix.ColumnCount; i++)
			for (int j = 0; j < matrix.RowCount; j++)
			{
				newMatrix[j, i] = func(matrix[j, i]);
			}
		return newMatrix;
	}

	public double Score(Func<GeneticNeuralNet, double> scoreFunc, int sampleSize)
	{
		return Enumerable.Range(0, sampleSize).Average(_ => scoreFunc(this));
	}

	public void Mutate(double epsilon, double mutationRate)
	{
		for (int i = 0; i < Weights.Count; i++)
		{
			Weights[i] = MatrixPerPoint(Weights[i], f =>
			{
				if (Utility.Random.NextDouble() > mutationRate) return f; // no mutation case

				var mutationScalar = Utility.Random.NextDouble() * 2 - 1;
				return f + (mutationScalar * epsilon);
			});
		}
	}

	public static void Write(BinaryWriter writer, GeneticNeuralNet net)
	{
		writer.Write(net.LayerDepths.Length);
		foreach (var layerDepth in net.LayerDepths)
			writer.Write(layerDepth);

		writer.Write(net.Weights.Count);
		foreach(var weight in net.Weights)
		{
			writer.Write(weight);
		}
	}
	public static GeneticNeuralNet Read(BinaryReader reader)
	{
		var layerDepthCount = reader.ReadInt32();
		int[] layerDepths = new int[layerDepthCount];
		for (int i = 0; i < layerDepthCount; i++)
			layerDepths[i] = reader.ReadInt32();

		var weightCount = reader.ReadInt32();
		var weights = new Matrix<double>[weightCount];
		for (int i = 0; i < weightCount; i++)
			weights[i] = reader.ReadMatrix();

		return new GeneticNeuralNet(weights.ToList(), layerDepths);
	}

	internal GeneticNeuralNet Clone()
	{
		return new GeneticNeuralNet(Weights.Select(w => w.Clone()).ToList(), LayerDepths);
	}
}
