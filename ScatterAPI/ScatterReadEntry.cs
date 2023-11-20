using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReDMA.ScatterAPI {
	/// <summary>
	///     Single scatter read.
	///     Use ScatterReadRound.AddEntry() to construct this class.
	/// </summary>
	public class ScatterReadEntry<T> : IScatterEntry {
#region Get Result

		/// <summary>
		///     Tries to return the Scatter Read Result.
		/// </summary>
		/// <typeparam name="TOut">Type to return.</typeparam>
		/// <param name="result">Result to populate.</param>
		/// <returns>True if successful, otherwise False.</returns>
		public Boolean TryGetResult<TOut>(out TOut result) {
			try {
				if (!this.IsFailed && this.Result is TOut tResult) {
					result = tResult;
					return true;
				}

				result = default;
				return false;
			}
			catch {
				result = default;
				return false;
			}
		}

#endregion

#region Properties

		/// <summary>
		///     Entry Index.
		/// </summary>
		public Int32 Index { get; set; }

		/// <summary>
		///     Entry ID.
		/// </summary>
		public Int32 Id { get; set; }

		/// <summary>
		///     Can be a ulong or another ScatterReadEntry.
		/// </summary>
		public Object Addr { get; set; }

		/// <summary>
		///     Offset to the Base Address.
		/// </summary>
		public UInt32 Offset { get; set; }

		/// <summary>
		///     Defines the type based on <typeparamref name="T" />
		/// </summary>
		public Type Type { get; } = typeof(T);

		/// <summary>
		///     Can be an int32 or another ScatterReadEntry.
		/// </summary>
		public Object Size { get; set; }

		/// <summary>
		///     True if the Scatter Read has failed.
		/// </summary>
		public Boolean IsFailed { get; set; }

		/// <summary>
		///     Scatter Read Result.
		/// </summary>
		protected T Result { get; set; }

#endregion

#region Read Prep

		/// <summary>
		///     Parses the address to read for this Scatter Read.
		///     Sets the Addr property for the object.
		/// </summary>
		/// <returns>Virtual address to read.</returns>
		public UInt64 ParseAddr() {
			UInt64 addr = 0x0;
			if (this.Addr is UInt64 p1) {
				addr = p1;
			}
			else if (this.Addr is MemPointer p2) {
				addr = p2;
			}
			else if (this.Addr is IScatterEntry ptrObj) // Check if the addr references another ScatterRead Result
			{
				if (ptrObj.TryGetResult(out MemPointer p3)) {
					addr = p3;
				}
				else {
					ptrObj.TryGetResult(out addr);
				}
			}

			this.Addr = addr;
			return addr;
		}

		/// <summary>
		///     (Base)
		///     Parses the number of bytes to read for this Scatter Read.
		///     Sets the Size property for the object.
		///     Derived classes should call upon this Base.
		/// </summary>
		/// <returns>Size of read.</returns>
		public virtual Int32 ParseSize() {
			Int32 size = 0;
			if (this.Type.IsValueType) {
				size = Unsafe.SizeOf<T>();
			}
			else if (this.Size is Int32 sizeInt) {
				size = sizeInt;
			}
			else if (this.Size is IScatterEntry sizeObj) // Check if the size references another ScatterRead Result
			{
				sizeObj.TryGetResult(out size);
			}

			this.Size = size;
			return size;
		}

#endregion

#region Set Result

		/// <summary>
		///     Sets the Result for this Scatter Read.
		/// </summary>
		/// <param name="buffer">Raw memory buffer for this read.</param>
		public void SetResult(Byte[] buffer) {
			try {
				if (this.IsFailed) {
					return;
				}

				if (this.Type.IsValueType) /// Value Type
				{
					this.SetValueResult(buffer);
				}
				else /// Ref Type
				{
					this.SetClassResult(buffer);
				}
			}
			catch {
				this.IsFailed = true;
			}
		}

		/// <summary>
		///     Set the Result from a Value Type.
		/// </summary>
		/// <param name="buffer">Raw memory buffer for this read.</param>
		private void SetValueResult(Byte[] buffer) {
			if (buffer.Length != Unsafe.SizeOf<T>()) // Safety Check
			{
				throw new ArgumentOutOfRangeException(nameof(buffer));
			}

			this.Result = Unsafe.As<Byte, T>(ref buffer[0]);
			if (this.Result is MemPointer memPtrResult) {
				memPtrResult.Validate();
			}
		}

		/// <summary>
		///     (Base)
		///     Set the Result from a Class Type.
		///     Derived classes should call upon this Base.
		/// </summary>
		/// <param name="buffer">Raw memory buffer for this read.</param>
		protected virtual void SetClassResult(Byte[] buffer) {
			if (this.Type == typeof(String)) {
				String value = Encoding.Default.GetString(buffer).Split('\0')[0];
				if (value is T result) // We already know the Types match, this is to satisfy the compiler
				{
					this.Result = result;
				}
			}
			else {
				throw new NotImplementedException(nameof(this.Type));
			}
		}

#endregion
	}
}