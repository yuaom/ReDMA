using System;

namespace ReDMA.ScatterAPI {
	public interface IScatterEntry {
		/// <summary>
		///     Entry Index.
		/// </summary>
		Int32 Index { get; }

		/// <summary>
		///     Entry ID.
		/// </summary>
		Int32 Id { get; }

		/// <summary>
		///     Can be a ulong or another ScatterReadEntry.
		/// </summary>
		Object Addr { get; set; }

		/// <summary>
		///     Offset to the Base Address.
		/// </summary>
		UInt32 Offset { get; }

		/// <summary>
		///     Defines the type based on <typeparamref name="T" />
		/// </summary>
		Type Type { get; }

		/// <summary>
		///     Can be an int32 or another ScatterReadEntry.
		/// </summary>
		Object Size { get; set; }

		/// <summary>
		///     True if the Scatter Read has failed.
		/// </summary>
		Boolean IsFailed { get; set; }

		/// <summary>
		///     Sets the Result for this Scatter Read.
		/// </summary>
		/// <param name="buffer">Raw memory buffer for this read.</param>
		void SetResult(Byte[] buffer);

		/// <summary>
		///     Parses the address to read for this Scatter Read.
		///     Sets the Addr property for the object.
		/// </summary>
		/// <returns>Virtual address to read.</returns>
		UInt64 ParseAddr();

		/// <summary>
		///     Parses the number of bytes to read for this Scatter Read.
		///     Sets the Size property for the object.
		/// </summary>
		/// <returns>Size of read.</returns>
		Int32 ParseSize();

		/// <summary>
		///     Tries to return the Scatter Read Result.
		/// </summary>
		/// <typeparam name="TOut">Type to return.</typeparam>
		/// <param name="result">Result to populate.</param>
		/// <returns>True if successful, otherwise False.</returns>
		Boolean TryGetResult<TOut>(out TOut result);
	}
}