using System;
using System.Collections.Generic;

namespace ReDMA.ScatterAPI {
	/// <summary>
	///     Provides mapping for a Scatter Read Operation.
	///     May contain multiple Scatter Read Rounds.
	/// </summary>
	public class ScatterReadMap {
		protected readonly Dictionary<Int32, Dictionary<Int32, IScatterEntry>> _results =
			new Dictionary<Int32, Dictionary<Int32, IScatterEntry>>();

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="indexCount">Number of indexes in the scatter read loop.</param>
		public ScatterReadMap(Int32 indexCount) {
			for (Int32 i = 0; i < indexCount; i++) {
				this._results.Add(i, new Dictionary<Int32, IScatterEntry>());
			}
		}

		protected List<ScatterReadRound> Rounds { get; } = new List<ScatterReadRound>();

		/// <summary>
		///     Contains results from Scatter Read after Execute() is performed. First key is Index, Second Key ID.
		/// </summary>
		public IReadOnlyDictionary<Int32, Dictionary<Int32, IScatterEntry>> Results {
			get {
				return this._results;
			}
		}

		/// <summary>
		///     Executes Scatter Read operation as defined per the map.
		/// </summary>
		public void Execute(MemDMA mem) {
			foreach (ScatterReadRound round in this.Rounds) {
				round.Run(mem);
			}
		}

		/// <summary>
		///     (Base)
		///     Add scatter read rounds to the operation. Each round is a successive scatter read, you may need multiple
		///     rounds if you have reads dependent on earlier scatter reads result(s).
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns></returns>
		public virtual ScatterReadRound AddRound(UInt32 pid, Boolean useCache = true) {
			ScatterReadRound round = new ScatterReadRound(pid, this._results, useCache);
			this.Rounds.Add(round);
			return round;
		}
	}
}