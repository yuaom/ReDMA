using System;
using System.Collections.Generic;

namespace ReDMA.ScatterAPI {
	/// <summary>
	///     Defines a Scatter Read Round. Each round will execute a single scatter read. If you have reads that
	///     are dependent on previous reads (chained pointers for example), you may need multiple rounds.
	/// </summary>
	public class ScatterReadRound {
		private readonly UInt32 _pid;
		private readonly Boolean _useCache;

		/// <summary>
		///     Do not use this constructor directly. Call .AddRound() from the ScatterReadMap.
		/// </summary>
		public ScatterReadRound(UInt32 pid, Dictionary<Int32, Dictionary<Int32, IScatterEntry>> results,
			Boolean useCache) {
			this._pid = pid;
			this.Results = results;
			this._useCache = useCache;
		}

		protected Dictionary<Int32, Dictionary<Int32, IScatterEntry>> Results { get; }
		protected List<IScatterEntry> Entries { get; } = new List<IScatterEntry>();

		/// <summary>
		///     (Base)
		///     Adds a single Scatter Read
		/// </summary>
		/// <param name="index">For loop index this is associated with.</param>
		/// <param name="id">Random ID number to identify the entry's purpose.</param>
		/// <param name="addr">
		///     Address to read from (you can pass a ScatterReadEntry from an earlier round,
		///     and it will use the result).
		/// </param>
		/// <param name="size">
		///     Size of oject to read (ONLY for reference types, value types get size from
		///     Type). You canc pass a ScatterReadEntry from an earlier round and it will use the Result.
		/// </param>
		/// <param name="offset">
		///     Optional offset to add to address (usually in the event that you pass a
		///     ScatterReadEntry to the Addr field).
		/// </param>
		/// <returns>The newly created ScatterReadEntry.</returns>
		public virtual ScatterReadEntry<T> AddEntry<T>(Int32 index, Int32 id, Object addr, Object size = null,
			UInt32 offset = 0x0) {
			ScatterReadEntry<T> entry = new ScatterReadEntry<T> {
				Index = index,
				Id = id,
				Addr = addr,
				Size = size,
				Offset = offset
			};
			this.Results[index].Add(id, entry);
			this.Entries.Add(entry);
			return entry;
		}

		/// <summary>
		///     ** Internal API use only do not use **
		/// </summary>
		internal void Run(MemDMA mem) {
			mem.ReadScatter(new Span<IScatterEntry>(this.Entries.ToArray()), this._useCache);
		}
	}
}