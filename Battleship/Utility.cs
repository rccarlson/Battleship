using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
	internal static class Utility
	{
		private static readonly Random Random = new();

		/// <summary>Fisher-Yates shuffle</summary>
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			if (list is null) return list;
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = Random.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}

		public static TimeSpan Profile(Action action)
		{
			var sw = Stopwatch.StartNew();
			action();
			sw.Stop();
			return sw.Elapsed;
		}
		public static double ProfileMs(Action action) => Profile(action).TotalMilliseconds;
	}
}
