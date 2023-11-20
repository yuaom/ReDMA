using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ReDMA.ScatterAPI;
using ReDMA.Vmmsharp;

namespace ReDMA {
	/// <summary>
	///     Base Memory Module.
	///     Can be inherited if you want to make your own implementation.
	/// </summary>
	public class MemDMA : IDisposable {
#region ScatterRead

		/// <summary>
		///     (Base)
		///     Performs multiple reads in one sequence, significantly faster than single reads.
		///     Designed to run without throwing unhandled exceptions, which will ensure the maximum amount of
		///     reads are completed OK even if a couple fail.
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="entries">Scatter Read Entries to read from for this round.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		internal virtual void ReadScatter(ReadOnlySpan<IScatterEntry> entries, Boolean useCache = true) {
			HashSet<UInt64> pagesToRead =
				new HashSet<UInt64>(); // Will contain each unique page only once to prevent reading the same page multiple times
			foreach (IScatterEntry entry in entries) // First loop through all entries - GET INFO
			{
				// Parse Address and Size properties
				UInt64 addr = entry.ParseAddr();
				UInt32 size = (UInt32)entry.ParseSize();

				// INTEGRITY CHECK - Make sure the read is valid
				if (addr == 0x0 || size == 0) {
					entry.IsFailed = true;
					continue;
				}

				// location of object
				UInt64 readAddress = addr + entry.Offset;
				// get the number of pages
				UInt32 numPages = ADDRESS_AND_SIZE_TO_SPAN_PAGES(readAddress, size);
				UInt64 basePage = PAGE_ALIGN(readAddress);

				//loop all the pages we would need
				for (Int32 p = 0; p < numPages; p++) {
					UInt64 page = basePage + PAGE_SIZE * (UInt32)p;
					pagesToRead.Add(page);
				}
			}

			UInt32 flags = useCache ? 0 : Vmm.FLAG_NOCACHE;
			MEM_SCATTER[]
				scatters = this.HVmm.MemReadScatter(this.Pid, flags, pagesToRead.ToArray()); // execute scatter read

			foreach (IScatterEntry entry in entries) // Second loop through all entries - PARSE RESULTS
			{
				if (entry.IsFailed) // Skip this entry, leaves result as null
				{
					continue;
				}

				UInt64 readAddress = (UInt64)entry.Addr + entry.Offset; // location of object
				UInt32 pageOffset = BYTE_OFFSET(readAddress); // Get object offset from the page start address

				UInt32 size = (UInt32)(Int32)entry.Size;
				Byte[] buffer = new Byte[size]; // Alloc result buffer on heap
				Int32 bytesCopied = 0; // track number of bytes copied to ensure nothing is missed
				UInt32 cb = Math.Min(size, (UInt32)PAGE_SIZE - pageOffset); // bytes to read this page

				UInt32 numPages =
					ADDRESS_AND_SIZE_TO_SPAN_PAGES(readAddress,
						size); // number of pages to read from (in case result spans multiple pages)
				UInt64 basePage = PAGE_ALIGN(readAddress);

				for (Int32 p = 0; p < numPages; p++) {
					UInt64 page = basePage + PAGE_SIZE * (UInt32)p; // get current page addr
					MEM_SCATTER scatter = scatters.FirstOrDefault(x => x.qwA == page); // retrieve page of mem needed
					if (scatter.f) // read succeeded -> copy to buffer
					{
						scatter.pb
							.AsSpan((Int32)pageOffset, (Int32)cb)
							.CopyTo(buffer.AsSpan(bytesCopied, (Int32)cb)); // Copy bytes to buffer
						bytesCopied += (Int32)cb;
					}
					else // read failed -> set failed flag
					{
						entry.IsFailed = true;
						break;
					}

					cb = (UInt32)PAGE_SIZE; // set bytes to read next page
					if (bytesCopied + cb > size) // partial chunk last page
					{
						cb = size - (UInt32)bytesCopied;
					}

					pageOffset = 0x0; // Next page (if any) should start at 0x0
				}

				if (bytesCopied != size) {
					entry.IsFailed = true;
				}

				entry.SetResult(buffer);
			}
		}

#endregion

#region WriteMethods

		/// <summary>
		///     (Base)
		///     Write value type/struct to specified address.
		/// </summary>
		/// <typeparam name="T">Value Type to write.</typeparam>
		/// <param name="pid">Process ID to write to.</param>
		/// <param name="addr">Virtual Address to write to.</param>
		/// <param name="value"></param>
		/// <exception cref="DMAException"></exception>
		public virtual void WriteValue<T>(UInt64 addr, T value)
			where T : unmanaged {
			try {
				if (!this.HVmm.MemWriteStruct(this.Pid, addr, value)) {
					throw new Exception("Memory Write Failed!");
				}
			}
			catch (Exception ex) {
				throw new DMAException($"[DMA] ERROR writing {typeof(T)} value at 0x{addr.ToString("X")}", ex);
			}
		}

#endregion

		public Boolean IsValid() {
			return this.HVmm != null && this.HVmm.PidList().Contains(this.Pid);
		}

#region Fields/Properties/Constructor

		private const String MemoryMapFile = "mmap.txt";

		/// <summary>
		///     MemProcFS Vmm Instance
		/// </summary>
		public Vmm HVmm { get; }

		public UInt32 Pid { get; set; }

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="args">(Optional) Custom Startup Args. If NULL default FPGA parameters will be used.</param>
		/// <param name="autoMemMap">Automatic Memory Map Generation/Initialization. (Default: True)</param>
		public MemDMA(String[] args = null, Boolean autoMemMap = true) {
			try {
				Debug.WriteLine("[DMA] Loading...");
				if (args is null) {
					args = new[] { "-printf", "-v", "-device", "fpga", "-waitinitialize" }; // Default args
				}

				if (autoMemMap && !File.Exists(MemoryMapFile)) {
					try {
						Debug.WriteLine("[DMA] Auto Mem Map");
						this.HVmm = new Vmm(args);
						Byte[] map = this.GetMemMap();
						File.WriteAllBytes(MemoryMapFile, map);
					}
					finally {
						this.HVmm?.Dispose(); // Close FPGA Connection after getting map.
						this.HVmm = null; // Null Vmm Handle
					}
				}

				if (autoMemMap) // Append Memory Map Args
				{
					String[] mapArgs = { "-memmap", MemoryMapFile };
					args = args.Concat(mapArgs).ToArray();
				}

				this.HVmm = new Vmm(args);
			}
			catch (Exception ex) {
				throw new DMAException("[DMA] INIT ERROR", ex);
			}
		}

#endregion

#region Mem Startup

		/// <summary>
		///     Generates a Physical Memory Map in ASCII Binary Format.
		///     https://github.com/ufrisk/LeechCore/wiki/Device_FPGA_AMD_Thunderbolt
		/// </summary>
		private Byte[] GetMemMap() {
			try {
				Vmm.MAP_PHYSMEMENTRY[] map = this.HVmm.Map_GetPhysMem();
				if (map.Length == 0) {
					throw new Exception("VMMDLL_Map_GetPhysMem FAIL!");
				}

				StringBuilder sb = new StringBuilder();
				Int32 leftLength = map.Max(x => x.pa).ToString("x").Length;
				for (Int32 i = 0; i < map.Length; i++) {
					sb.AppendFormat($"{{0,{-leftLength}}}", map[i].pa.ToString("x"))
						.Append($" - {(map[i].pa + map[i].cb - 1).ToString("x")}")
						.AppendLine();
				}

				return Encoding.ASCII.GetBytes(sb.ToString());
			}
			catch (Exception ex) {
				throw new DMAException("[DMA] Unable to get Mem Map!", ex);
			}
		}

		/// <summary>
		///     Obtain the PID for a process.
		/// </summary>
		/// <param name="process">Process Name (including file extension, ex: .exe)</param>
		/// <returns>Process ID (PID)</returns>
		public void GetPid(String process) {
			if (!this.HVmm.PidGetFromName(process, out UInt32 pid)) {
				throw new DMAException("PID Lookup Failed");
			}

			this.Pid = pid;
		}

		/// <summary>
		///     Obtain the Base Address of a Process Module.
		/// </summary>
		/// <param name="pid">Process ID the Module is contained in.</param>
		/// <param name="module">Module Name (including file extension, ex: .dll)</param>
		/// <returns>Module Base virtual address.</returns>
		public UInt64 GetModuleBase(String module) {
			UInt64 moduleBase = this.HVmm.ProcessGetModuleBase(this.Pid, module);
			if (moduleBase == 0x0) {
				throw new DMAException($"Unable to get Module Base for '{module}'");
			}

			return moduleBase;
		}

#endregion

#region ReadMethods

		/// <summary>
		///     (Base)
		///     Read memory into a buffer.
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="addr">Virtual Address to read from.</param>
		/// <param name="size">Size (bytes) of this read.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns>Byte Array containing memory read.</returns>
		/// <exception cref="DMAException"></exception>
		public virtual Byte[] ReadBuffer(UInt64 addr, Int32 size, Boolean useCache = true) {
			try {
				UInt32 flags = useCache ? 0 : Vmm.FLAG_NOCACHE;
				Byte[] buf = this.HVmm.MemRead(this.Pid, addr, (UInt32)size, flags);
				if (buf.Length != size) {
					throw new DMAException("Incomplete memory read!");
				}

				return buf;
			}
			catch (Exception ex) {
				throw new DMAException($"[DMA] ERROR reading buffer at 0x{addr.ToString("X")}", ex);
			}
		}

		/// <summary>
		///     (Base)
		///     Read a chain of pointers and get the final result.
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="addr">Virtual Address to read from.</param>
		/// <param name="offsets">Offsets to read in a chain.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns>Virtual address of the Pointer Result.</returns>
		/// <exception cref="DMAException"></exception>
		public virtual UInt64 ReadPtrChain(UInt64 addr, UInt32[] offsets, Boolean useCache = true) {
			UInt64 ptr = addr; // push ptr to first address value
			for (Int32 i = 0; i < offsets.Length; i++) {
				try {
					ptr = this.ReadPtr(ptr + offsets[i], useCache);
				}
				catch (Exception ex) {
					throw new DMAException(
						$"[DMA] ERROR reading pointer chain at index {i}, addr 0x{ptr.ToString("X")} + 0x{offsets[i].ToString("X")}",
						ex);
				}
			}

			return ptr;
		}

		/// <summary>
		///     (Base)
		///     Resolves a pointer and returns the memory address it points to.
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="addr">Virtual Address to read from.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns>Virtual address of the Pointer Result.</returns>
		/// <exception cref="DMAException"></exception>
		public virtual UInt64 ReadPtr(UInt64 addr, Boolean useCache = true) {
			try {
				MemPointer ptr = this.ReadValue<MemPointer>(addr, useCache);
				ptr.Validate();
				return ptr;
			}
			catch (Exception ex) {
				throw new DMAException($"[DMA] ERROR reading pointer at 0x{addr.ToString("X")}", ex);
			}
		}

		/// <summary>
		///     (Base)
		///     Read value type/struct from specified address.
		/// </summary>
		/// <typeparam name="T">Value Type to read.</typeparam>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="addr">Virtual Address to read from.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns>Value Type of <typeparamref name="T" /></returns>
		/// <exception cref="DMAException"></exception>
		public virtual T ReadValue<T>(UInt64 addr, Boolean useCache = true)
			where T : unmanaged {
			try {
				UInt32 flags = useCache ? 0 : Vmm.FLAG_NOCACHE;
				if (!this.HVmm.MemReadStruct(this.Pid, addr, out T result, flags)) {
					throw new Exception("Memory Read Failed!");
				}

				return result;
			}
			catch (Exception ex) {
				throw new DMAException($"[DMA] ERROR reading {typeof(T)} value at 0x{addr.ToString("X")}", ex);
			}
		}

		/// <summary>
		///     (Base)
		///     Read null terminated string (UTF-8).
		/// </summary>
		/// <param name="pid">Process ID to read from.</param>
		/// <param name="addr">Virtual Address to read from.</param>
		/// <param name="size">Size (bytes) of this read.</param>
		/// <param name="useCache">Use caching for this read (recommended).</param>
		/// <returns>UTF-8 Encoded String</returns>
		/// <exception cref="DMAException"></exception>
		public virtual String ReadString(UInt64 addr, UInt32 size, Boolean useCache = true) // read n bytes (string)
		{
			try {
				UInt32 flags = useCache ? 0 : Vmm.FLAG_NOCACHE;
				Byte[] buf = this.HVmm.MemRead(this.Pid, addr, size, flags);
				return Encoding.UTF8.GetString(buf).Split('\0')[0];
			}
			catch (Exception ex) {
				throw new DMAException($"[DMA] ERROR reading string at 0x{addr.ToString("X")}", ex);
			}
		}

#endregion

#region IDisposable

		private readonly Object _disposeSync = new Object();
		private Boolean _disposed;

		/// <summary>
		///     Closes the FPGA Connection and cleans up native resources.
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
		}

		// Public Dispose Pattern
		protected virtual void Dispose(Boolean disposing) {
			lock (this._disposeSync) {
				if (!this._disposed) {
					if (disposing) {
						this.HVmm.Dispose();
					}

					this._disposed = true;
				}
			}
		}

#endregion

#region Memory Macros

		/// Mem Align Functions Ported from Win32 (C Macros)
		protected const UInt64 PAGE_SIZE = 0x1000;

		private const Int32 PAGE_SHIFT = 12;

		/// <summary>
		///     The PAGE_ALIGN macro takes a virtual address and returns a page-aligned
		///     virtual address for that page.
		/// </summary>
		/// <param name="va">Virtual address to check.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static UInt64 PAGE_ALIGN(UInt64 va) {
			return va & ~(PAGE_SIZE - 1);
		}

		/// <summary>
		///     The ADDRESS_AND_SIZE_TO_SPAN_PAGES macro takes a virtual address and size and returns the number of pages spanned
		///     by the size.
		/// </summary>
		/// <param name="va">Virtual Address to check.</param>
		/// <param name="size">Size of Memory Chunk spanned by this virtual address.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static UInt32 ADDRESS_AND_SIZE_TO_SPAN_PAGES(UInt64 va, UInt32 size) {
			return (UInt32)((BYTE_OFFSET(va) + size + (PAGE_SIZE - 1)) >> PAGE_SHIFT);
		}

		/// <summary>
		///     The BYTE_OFFSET macro takes a virtual address and returns the byte offset
		///     of that address within the page.
		/// </summary>
		/// <param name="va">Virtual Address to get the byte offset of.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static UInt32 BYTE_OFFSET(UInt64 va) {
			return (UInt32)(va & (PAGE_SIZE - 1));
		}

#endregion
	}

#region Exceptions

	public sealed class DMAException : Exception {
		public DMAException() {
		}

		public DMAException(String message)
			: base(message) {
		}

		public DMAException(String message, Exception inner)
			: base(message, inner) {
		}
	}

	public sealed class NullPtrException : Exception {
		public NullPtrException() {
		}

		public NullPtrException(String message)
			: base(message) {
		}

		public NullPtrException(String message, Exception inner)
			: base(message, inner) {
		}

#endregion
	}
}