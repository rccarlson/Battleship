using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipNeuralNet;

internal static  class Utility
{
	internal static readonly Random Random = new();
	public static IEnumerable<(T, S)> Fold<T, S>(this IEnumerable<T> source, IEnumerable<S> source2)
	{
		var enumerator1 = source.GetEnumerator();
		var enumerator2 = source2.GetEnumerator();

		while (enumerator1.MoveNext() && enumerator2.MoveNext())
		{
			yield return (enumerator1.Current, enumerator2.Current);
		}
		if (enumerator2.MoveNext()) throw new InvalidDataException($"Not enough elements in first {nameof(IEnumerable<S>)}");
		if (enumerator1.MoveNext()) throw new InvalidDataException($"Not enough elements in second {nameof(IEnumerable<S>)}");
	}

	public static T TakeRandom<T>(this IList<T> source)
	{
		var idx = Random.Next(source.Count);
		return (T)source[idx];
	}

	public static void Write(this BinaryWriter writer, Matrix<double> matrix)
	{
		var arr = matrix.ToColumnMajorArray();
		writer.Write(matrix.RowCount);
		writer.Write(matrix.ColumnCount);
		writer.Write(arr.Length);
		foreach (var item in arr)
		{
			writer.Write(item);
		}
	}
	public static Matrix<double> ReadMatrix(this BinaryReader reader)
	{
		var rows = reader.ReadInt32();
		var columns = reader.ReadInt32();
		var arrLen = reader.ReadInt32();
		double[] arr = new double[arrLen];
		for (int i = 0; i < arrLen; i++) arr[i] = reader.ReadDouble();
		var matrix = Matrix<double>.Build.DenseOfColumnMajor(rows, columns, arr);
		return matrix;
	}
}
