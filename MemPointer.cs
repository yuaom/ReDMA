using System;
using System.Runtime.CompilerServices;

namespace ReDMA {
	/// <summary>
	///     Represents a 64-Bit Unsigned Pointer Address.
	/// </summary>
	public readonly struct MemPointer {
		public MemPointer(UInt64 value) {
			this.Va = value;
		}

		public static implicit operator MemPointer(UInt64 x) {
			return new MemPointer(x);
		}

		public static implicit operator UInt64(MemPointer x) {
			return x.Va;
		}

		/// <summary>
		///     Virtual Address of this Pointer.
		/// </summary>
		public readonly UInt64 Va;

		/// <summary>
		///     Validates the Pointer.
		/// </summary>
		/// <exception cref="NullPtrException"></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Validate() {
			if (this.Va == 0x0) {
				throw new NullPtrException();
			}
		}

		/// <summary>
		///     Convert to string format.
		/// </summary>
		/// <returns>Pointer Address represented in Upper-Case Hex.</returns>
		public override String ToString() {
			return this.Va.ToString("X");
		}
	}
}