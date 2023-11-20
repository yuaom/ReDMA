using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

/*
 *  C# API wrapper 'vmmsharp' for MemProcFS 'vmm.dll' and LeechCore 'leechcore.dll' APIs.
 *
 *  Please see the example project in vmm_example.cs for additional information.
 *
 *  Please consult the C/C++ header files vmmdll.h and leechcore.h for information about
 *  parameters and API usage.
 *
 *  (c) Ulf Frisk, 2020-2023
 *  Author: Ulf Frisk, pcileech@frizk.net
 *
 *  Version 5.8
 *
 */
namespace ReDMA.Vmmsharp {
	public struct MEM_SCATTER {
		public Boolean f;
		public UInt64 qwA;
		public Byte[] pb;
	}

// LeechCore public API:
	public sealed class LeechCore : IDisposable {
		//---------------------------------------------------------------------
		// LEECHCORE: PUBLIC API CONSTANTS BELOW:
		//---------------------------------------------------------------------
		public const UInt32 LC_CONFIG_VERSION = 0xc0fd0002;
		public const UInt32 LC_CONFIG_ERRORINFO_VERSION = 0xc0fe0002;

		public const UInt32 LC_CONFIG_PRINTF_ENABLED = 0x01;
		public const UInt32 LC_CONFIG_PRINTF_V = 0x02;
		public const UInt32 LC_CONFIG_PRINTF_VV = 0x04;
		public const UInt32 LC_CONFIG_PRINTF_VVV = 0x08;

		public const UInt64 LC_OPT_CORE_PRINTF_ENABLE = 0x4000000100000000; // RW
		public const UInt64 LC_OPT_CORE_VERBOSE = 0x4000000200000000; // RW
		public const UInt64 LC_OPT_CORE_VERBOSE_EXTRA = 0x4000000300000000; // RW
		public const UInt64 LC_OPT_CORE_VERBOSE_EXTRA_TLP = 0x4000000400000000; // RW
		public const UInt64 LC_OPT_CORE_VERSION_MAJOR = 0x4000000500000000; // R
		public const UInt64 LC_OPT_CORE_VERSION_MINOR = 0x4000000600000000; // R
		public const UInt64 LC_OPT_CORE_VERSION_REVISION = 0x4000000700000000; // R
		public const UInt64 LC_OPT_CORE_ADDR_MAX = 0x1000000800000000; // R
		public const UInt64 LC_OPT_CORE_STATISTICS_CALL_COUNT = 0x4000000900000000; // R [lo-dword: LC_STATISTICS_ID_*]
		public const UInt64 LC_OPT_CORE_STATISTICS_CALL_TIME = 0x4000000a00000000; // R [lo-dword: LC_STATISTICS_ID_*]
		public const UInt64 LC_OPT_CORE_VOLATILE = 0x1000000b00000000; // R
		public const UInt64 LC_OPT_CORE_READONLY = 0x1000000c00000000; // R

		public const UInt64 LC_OPT_MEMORYINFO_VALID = 0x0200000100000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_FLAG_32BIT = 0x0200000300000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_FLAG_PAE = 0x0200000400000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_ARCH = 0x0200001200000000; // R - LC_ARCH_TP
		public const UInt64 LC_OPT_MEMORYINFO_OS_VERSION_MINOR = 0x0200000500000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_VERSION_MAJOR = 0x0200000600000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_DTB = 0x0200000700000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_PFN = 0x0200000800000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_PsLoadedModuleList = 0x0200000900000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_PsActiveProcessHead = 0x0200000a00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_MACHINE_IMAGE_TP = 0x0200000b00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_NUM_PROCESSORS = 0x0200000c00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_SYSTEMTIME = 0x0200000d00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_UPTIME = 0x0200000e00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_KERNELBASE = 0x0200000f00000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_KERNELHINT = 0x0200001000000000; // R
		public const UInt64 LC_OPT_MEMORYINFO_OS_KdDebuggerDataBlock = 0x0200001100000000; // R

		public const UInt64 LC_OPT_FPGA_PROBE_MAXPAGES = 0x0300000100000000; // RW
		public const UInt64 LC_OPT_FPGA_MAX_SIZE_RX = 0x0300000300000000; // RW
		public const UInt64 LC_OPT_FPGA_MAX_SIZE_TX = 0x0300000400000000; // RW
		public const UInt64 LC_OPT_FPGA_DELAY_PROBE_READ = 0x0300000500000000; // RW - uS
		public const UInt64 LC_OPT_FPGA_DELAY_PROBE_WRITE = 0x0300000600000000; // RW - uS
		public const UInt64 LC_OPT_FPGA_DELAY_WRITE = 0x0300000700000000; // RW - uS
		public const UInt64 LC_OPT_FPGA_DELAY_READ = 0x0300000800000000; // RW - uS
		public const UInt64 LC_OPT_FPGA_RETRY_ON_ERROR = 0x0300000900000000; // RW
		public const UInt64 LC_OPT_FPGA_DEVICE_ID = 0x0300008000000000; // RW - bus:dev:fn (ex: 04:00.0 === 0x0400).
		public const UInt64 LC_OPT_FPGA_FPGA_ID = 0x0300008100000000; // R
		public const UInt64 LC_OPT_FPGA_VERSION_MAJOR = 0x0300008200000000; // R
		public const UInt64 LC_OPT_FPGA_VERSION_MINOR = 0x0300008300000000; // R

		public const UInt64
			LC_OPT_FPGA_ALGO_TINY = 0x0300008400000000; // RW - 1/0 use tiny 128-byte/tlp read algorithm.

		public const UInt64
			LC_OPT_FPGA_ALGO_SYNCHRONOUS = 0x0300008500000000; // RW - 1/0 use synchronous (old) read algorithm.

		public const UInt64
			LC_OPT_FPGA_CFGSPACE_XILINX =
				0x0300008600000000; // RW - [lo-dword: register address in bytes] [bytes: 0-3: data, 4-7: byte_enable(if wr/set); top bit = cfg_mgmt_wr_rw1c_as_rw]

		public const UInt64
			LC_OPT_FPGA_TLP_READ_CB_WITHINFO =
				0x0300009000000000; // RW - 1/0 call TLP read callback with additional string info in szInfo

		public const UInt64
			LC_OPT_FPGA_TLP_READ_CB_FILTERCPL =
				0x0300009100000000; // RW - 1/0 call TLP read callback with memory read completions from read calls filtered

		public const UInt64 LC_CMD_FPGA_PCIECFGSPACE = 0x0000010300000000; // R
		public const UInt64 LC_CMD_FPGA_CFGREGPCIE = 0x0000010400000000; // RW - [lo-dword: register address]
		public const UInt64 LC_CMD_FPGA_CFGREGCFG = 0x0000010500000000; // RW - [lo-dword: register address]
		public const UInt64 LC_CMD_FPGA_CFGREGDRP = 0x0000010600000000; // RW - [lo-dword: register address]

		public const UInt64
			LC_CMD_FPGA_CFGREGCFG_MARKWR =
				0x0000010700000000; // W  - write with mask [lo-dword: register address] [bytes: 0-1: data, 2-3: mask]

		public const UInt64
			LC_CMD_FPGA_CFGREGPCIE_MARKWR =
				0x0000010800000000; // W  - write with mask [lo-dword: register address] [bytes: 0-1: data, 2-3: mask]

		public const UInt64 LC_CMD_FPGA_CFGREG_DEBUGPRINT = 0x0000010a00000000; // N/A
		public const UInt64 LC_CMD_FPGA_PROBE = 0x0000010b00000000; // RW
		public const UInt64 LC_CMD_FPGA_CFGSPACE_SHADOW_RD = 0x0000010c00000000; // R

		public const UInt64
			LC_CMD_FPGA_CFGSPACE_SHADOW_WR = 0x0000010d00000000; // W  - [lo-dword: config space write base address]

		public const UInt64 LC_CMD_FPGA_TLP_WRITE_SINGLE = 0x0000011000000000; // W  - write single tlp BYTE:s
		public const UInt64 LC_CMD_FPGA_TLP_WRITE_MULTIPLE = 0x0000011100000000; // W  - write multiple LC_TLP:s

		public const UInt64
			LC_CMD_FPGA_TLP_TOSTRING =
				0x0000011200000000; // RW - convert single TLP to LPSTR; *pcbDataOut includes NULL terminator.

		public const UInt64
			LC_CMD_FPGA_TLP_CONTEXT =
				0x2000011400000000; // W - set/unset TLP user-defined context to be passed to callback function. (pbDataIn == LPVOID user context). [not remote].

		public const UInt64
			LC_CMD_FPGA_TLP_CONTEXT_RD =
				0x2000011b00000000; // R - get TLP user-defined context to be passed to callback function. [not remote].

		public const UInt64
			LC_CMD_FPGA_TLP_FUNCTION_CALLBACK =
				0x2000011500000000; // W - set/unset TLP callback function (pbDataIn == PLC_TLP_CALLBACK). [not remote].

		public const UInt64
			LC_CMD_FPGA_TLP_FUNCTION_CALLBACK_RD = 0x2000011c00000000; // R - get TLP callback function. [not remote].

		public const UInt64
			LC_CMD_FPGA_BAR_CONTEXT =
				0x2000011800000000; // W - set/unset BAR user-defined context to be passed to callback function. (pbDataIn == LPVOID user context). [not remote].

		public const UInt64
			LC_CMD_FPGA_BAR_CONTEXT_RD =
				0x2000011d00000000; // R - get BAR user-defined context to be passed to callback function. [not remote].

		public const UInt64
			LC_CMD_FPGA_BAR_FUNCTION_CALLBACK =
				0x2000011900000000; // W - set/unset BAR callback function (pbDataIn == PLC_BAR_CALLBACK). [not remote].

		public const UInt64
			LC_CMD_FPGA_BAR_FUNCTION_CALLBACK_RD = 0x2000011e00000000; // R - get BAR callback function. [not remote].

		public const UInt64
			LC_CMD_FPGA_BAR_INFO = 0x0000011a00000000; // R - get BAR info (pbDataOut == LC_BAR_INFO[6]).

		public const UInt64 LC_CMD_FILE_DUMPHEADER_GET = 0x0000020100000000; // R

		public const UInt64 LC_CMD_STATISTICS_GET = 0x4000010000000000; // R
		public const UInt64 LC_CMD_MEMMAP_GET = 0x4000020000000000; // R  - MEMMAP as LPSTR
		public const UInt64 LC_CMD_MEMMAP_SET = 0x4000030000000000; // W  - MEMMAP as LPSTR
		public const UInt64 LC_CMD_MEMMAP_GET_STRUCT = 0x4000040000000000; // R  - MEMMAP as LC_MEMMAP_ENTRY[]
		public const UInt64 LC_CMD_MEMMAP_SET_STRUCT = 0x4000050000000000; // W  - MEMMAP as LC_MEMMAP_ENTRY[]

		public const UInt64 LC_CMD_AGENT_EXEC_PYTHON = 0x8000000100000000; // RW - [lo-dword: optional timeout in ms]
		public const UInt64 LC_CMD_AGENT_EXIT_PROCESS = 0x8000000200000000; //    - [lo-dword: process exit code]
		public const UInt64 LC_CMD_AGENT_VFS_LIST = 0x8000000300000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_READ = 0x8000000400000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_WRITE = 0x8000000500000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_OPT_GET = 0x8000000600000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_OPT_SET = 0x8000000700000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_INITIALIZE = 0x8000000800000000; // RW
		public const UInt64 LC_CMD_AGENT_VFS_CONSOLE = 0x8000000900000000; // RW

		private Boolean disposed;
		private IntPtr hLC = IntPtr.Zero;

		// private zero-argument constructor - do not use!
		private LeechCore() {
		}

		private LeechCore(IntPtr hLC) {
			this.hLC = hLC;
		}

		public LeechCore(String strDevice) {
			LC_CONFIG cfg = new LC_CONFIG();
			cfg.dwVersion = LC_CONFIG_VERSION;
			cfg.szDevice = strDevice;
			IntPtr hLC = lci.LcCreate(ref cfg);
			if (hLC == IntPtr.Zero) {
				throw new Exception("LeechCore: failed to create object.");
			}

			this.hLC = hLC;
		}

		public LeechCore(String strDevice, String strRemote, UInt32 dwVerbosityFlags, UInt64 paMax) {
			LC_CONFIG cfg = new LC_CONFIG();
			cfg.dwVersion = LC_CONFIG_VERSION;
			cfg.szDevice = strDevice;
			cfg.szRemote = strRemote;
			cfg.dwPrintfVerbosity = dwVerbosityFlags;
			cfg.paMax = paMax;
			IntPtr hLC = lci.LcCreate(ref cfg);
			if (hLC == IntPtr.Zero) {
				throw new Exception("LeechCore: failed to create object.");
			}

			this.hLC = hLC;
		}

		public LeechCore(Vmm vmm) {
			UInt64 pqwValue;
			if (!vmm.ConfigGet(Vmm.OPT_CORE_LEECHCORE_HANDLE, out pqwValue)) {
				throw new Exception("LeechCore: failed retrieving handle from Vmm.");
			}

			String strDevice = String.Format("existing://0x{0:X}", pqwValue);
			LC_CONFIG cfg = new LC_CONFIG();
			cfg.dwVersion = LC_CONFIG_VERSION;
			cfg.szDevice = strDevice;
			IntPtr hLC = lci.LcCreate(ref cfg);
			if (hLC == IntPtr.Zero) {
				throw new Exception("LeechCore: failed to create object.");
			}

			this.hLC = hLC;
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Factory method creating a new LeechCore object taking a LC_CONFIG structure
		// containing the configuration and optionally return a LC_CONFIG_ERRORINFO
		// structure containing any error.
		// Use this when you wish to gain greater control of creating LeechCore objects.
		public static LeechCore CreateFromConfig(ref LC_CONFIG pLcCreateConfig,
			out LC_CONFIG_ERRORINFO ConfigErrorInfo) {
			IntPtr pLcErrorInfo;
			Int32 cbERROR_INFO = Marshal.SizeOf<lci.LC_CONFIG_ERRORINFO>();
			IntPtr hLC = lci.LcCreateEx(ref pLcCreateConfig, out pLcErrorInfo);
			ConfigErrorInfo = new LC_CONFIG_ERRORINFO();
			ConfigErrorInfo.strUserText = "";
			if (pLcErrorInfo != IntPtr.Zero && hLC != IntPtr.Zero) {
				return new LeechCore(hLC);
			}

			if (hLC != IntPtr.Zero) {
				lci.LcClose(hLC);
			}

			if (pLcErrorInfo != IntPtr.Zero) {
				lci.LC_CONFIG_ERRORINFO e = Marshal.PtrToStructure<lci.LC_CONFIG_ERRORINFO>(pLcErrorInfo);
				if (e.dwVersion == LC_CONFIG_ERRORINFO_VERSION) {
					ConfigErrorInfo.fValid = true;
					ConfigErrorInfo.fUserInputRequest = e.fUserInputRequest;
					if (e.cwszUserText > 0) {
						ConfigErrorInfo.strUserText =
							Marshal.PtrToStringUni((IntPtr)(pLcErrorInfo.ToInt64() + cbERROR_INFO));
					}
				}

				lci.LcMemFree(pLcErrorInfo);
			}

			return null;
		}

		~LeechCore() {
			this.Dispose(false);
		}

		private void Dispose(Boolean disposing) {
			if (!this.disposed) {
				lci.LcClose(this.hLC);
				this.hLC = IntPtr.Zero;
				this.disposed = true;
			}
		}

		public void Close() {
			this.Dispose(true);
		}


		//---------------------------------------------------------------------
		// LEECHCORE: GENERAL FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		/// <summary>
		///     Read a single physical memory range.
		/// </summary>
		/// <param name="pa">Physical address to read.</param>
		/// <param name="cb">Number of bytes to read.</param>
		/// <returns>Bytes read.</returns>
		public Byte[] Read(UInt64 pa, UInt32 cb) {
			unsafe {
				Byte[] data = new Byte[cb];
				fixed (Byte* pb = data) {
					Boolean result = lci.LcRead(this.hLC, pa, cb, pb);
					return result ? data : null;
				}
			}
		}

		/// <summary>
		///     Read multiple page-sized physical memory ranges.
		/// </summary>
		/// <param name="pa">Array of multiple physical addresses to read.</param>
		/// <returns>An arary of MEM_SCATTER containing the page-sized results of the reads.</returns>
		public MEM_SCATTER[] ReadScatter(params UInt64[] pas) {
			Int32 i;
			Int64 vappMEMs, vapMEM;
			IntPtr pMEM, pMEM_qwA, pppMEMs;
			if (!lci.LcAllocScatter1((UInt32)pas.Length, out pppMEMs)) {
				return null;
			}

			vappMEMs = pppMEMs.ToInt64();
			for (i = 0; i < pas.Length; i++) {
				vapMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8)).ToInt64();
				pMEM_qwA = new IntPtr(vapMEM + 8);
				Marshal.WriteInt64(pMEM_qwA, (Int64)(pas[i] & ~(UInt64)0xfff));
			}

			MEM_SCATTER[] MEMs = new MEM_SCATTER[pas.Length];
			lci.LcReadScatter(this.hLC, (UInt32)MEMs.Length, pppMEMs);
			for (i = 0; i < MEMs.Length; i++) {
				pMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8));
				lci.LC_MEM_SCATTER n = Marshal.PtrToStructure<lci.LC_MEM_SCATTER>(pMEM);
				MEMs[i].f = n.f;
				MEMs[i].qwA = n.qwA;
				MEMs[i].pb = new Byte[0x1000];
				Marshal.Copy(n.pb, MEMs[i].pb, 0, 0x1000);
			}

			lci.LcMemFree(pppMEMs);
			return MEMs;
		}

		/// <summary>
		///     Write a single range of physical memory. The write is best-effort and may fail. It's recommended to verify the
		///     write with a subsequent read.
		/// </summary>
		/// <param name="pa">Physical address to write</param>
		/// <param name="data">Data to write starting at pa.</param>
		/// <returns></returns>
		public Boolean Write(UInt64 pa, Byte[] data) {
			unsafe {
				fixed (Byte* pb = data) {
					return lci.LcWrite(this.hLC, pa, (UInt32)data.Length, pb);
				}
			}
		}

		/// <summary>
		///     Write multiple page-sized physical memory ranges. The write is best-effort and may fail. It's recommended to verify
		///     the writes with subsequent reads.
		/// </summary>
		/// <param name="MEMs">MEMs containing the memory addresses and data to write.</param>
		public void WriteScatter(ref MEM_SCATTER[] MEMs) {
			Int32 i;
			Int64 vappMEMs, vapMEM;
			IntPtr pMEM, pMEM_f, pMEM_qwA, pMEM_pb, pppMEMs;
			for (i = 0; i < MEMs.Length; i++) {
				if (MEMs[i].pb == null || MEMs[i].pb.Length != 0x1000) {
					return;
				}
			}

			if (!lci.LcAllocScatter1((UInt32)MEMs.Length, out pppMEMs)) {
				return;
			}

			vappMEMs = pppMEMs.ToInt64();
			for (i = 0; i < MEMs.Length; i++) {
				vapMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8)).ToInt64();
				pMEM_f = new IntPtr(vapMEM + 4);
				pMEM_qwA = new IntPtr(vapMEM + 8);
				pMEM_pb = Marshal.ReadIntPtr(new IntPtr(vapMEM + 16));
				Marshal.WriteInt32(pMEM_f, MEMs[i].f ? 1 : 0);
				Marshal.WriteInt64(pMEM_qwA, (Int64)(MEMs[i].qwA & ~(UInt64)0xfff));
				Marshal.Copy(MEMs[i].pb, 0, pMEM_pb, MEMs[i].pb.Length);
			}

			lci.LcWriteScatter(this.hLC, (UInt32)MEMs.Length, pppMEMs);
			for (i = 0; i < MEMs.Length; i++) {
				pMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8));
				lci.LC_MEM_SCATTER n = Marshal.PtrToStructure<lci.LC_MEM_SCATTER>(pMEM);
				MEMs[i].f = n.f;
				MEMs[i].qwA = n.qwA;
			}

			lci.LcMemFree(pppMEMs);
		}

		/// <summary>
		///     Retrieve a LeechCore option value.
		/// </summary>
		/// <param name="fOption">Parameter LeechCore.LC_OPT_*</param>
		/// <param name="pqwValue">The option value retrieved.</param>
		/// <returns></returns>
		public Boolean GetOption(UInt64 fOption, out UInt64 pqwValue) {
			return lci.GetOption(this.hLC, fOption, out pqwValue);
		}

		/// <summary>
		///     Set a LeechCore option value.
		/// </summary>
		/// <param name="fOption">Parameter LeechCore.LC_OPT_*</param>
		/// <param name="qwValue">The option value to set.</param>
		/// <returns></returns>
		public Boolean SetOption(UInt64 fOption, UInt64 qwValue) {
			return lci.SetOption(this.hLC, fOption, qwValue);
		}

		/// <summary>
		///     Send a command to LeechCore.
		/// </summary>
		/// <param name="fOption">Parameter LeechCore.LC_CMD_*</param>
		/// <param name="DataIn">The data to set (or null).</param>
		/// <param name="DataOut">The data retrieved.</param>
		/// <returns></returns>
		public Boolean Command(UInt64 fOption, Byte[] DataIn, out Byte[] DataOut) {
			unsafe {
				Boolean result;
				UInt32 cbDataOut;
				IntPtr PtrDataOut;
				DataOut = null;
				if (DataIn == null) {
					result = lci.LcCommand(this.hLC, fOption, 0, null, out PtrDataOut, out cbDataOut);
				}
				else {
					fixed (Byte* pbDataIn = DataIn) {
						result = lci.LcCommand(this.hLC, fOption, (UInt32)DataIn.Length, pbDataIn, out PtrDataOut,
							out cbDataOut);
					}
				}

				if (!result) {
					return false;
				}

				DataOut = new Byte[cbDataOut];
				if (cbDataOut > 0) {
					Marshal.Copy(PtrDataOut, DataOut, 0, (Int32)cbDataOut);
					lci.LcMemFree(PtrDataOut);
				}

				return true;
			}
		}

		/// <summary>
		///     Retrieve the memory map currently in use by LeechCore.
		/// </summary>
		/// <returns>The memory map (or null on failure).</returns>
		public String GetMemMap() {
			Byte[] bMemMap;
			if (this.Command(LC_CMD_MEMMAP_GET, null, out bMemMap) && bMemMap.Length > 0) {
				return Encoding.UTF8.GetString(bMemMap);
			}

			return "";
		}

		/// <summary>
		///     Set the memory map for LeechCore to use.
		/// </summary>
		/// <param name="sMemMap">The memory map to set.</param>
		/// <returns></returns>
		public Boolean SetMemMap(String sMemMap) {
			return this.Command(LC_CMD_MEMMAP_SET, Encoding.UTF8.GetBytes(sMemMap), out Byte[] bMemMap);
		}


		//---------------------------------------------------------------------
		// LEECHCORE: CORE FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct LC_CONFIG {
			public UInt32 dwVersion;
			public UInt32 dwPrintfVerbosity;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szDevice;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szRemote;

			public IntPtr pfn_printf_opt;
			public UInt64 paMax;
			public Boolean fVolatile;
			public Boolean fWritable;
			public Boolean fRemote;
			public Boolean fRemoteDisableCompress;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szDeviceName;
		}

		public struct LC_CONFIG_ERRORINFO {
			public Boolean fValid;
			public Boolean fUserInputRequest;
			public String strUserText;
		}
	}

// MemProcFS public API:
	public sealed class Vmm : IDisposable {
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate Boolean VfsCallBack_AddDirectory(UInt64 h, [MarshalAs(UnmanagedType.LPUTF8Str)] String wszName,
			IntPtr pExInfo);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate Boolean VfsCallBack_AddFile(UInt64 h, [MarshalAs(UnmanagedType.LPUTF8Str)] String wszName,
			UInt64 cb, IntPtr pExInfo);

		public enum MAP_PFN_TYPE {
			Zero = 0,
			Free = 1,
			Standby = 2,
			Modified = 3,
			ModifiedNoWrite = 4,
			Bad = 5,
			Active = 6,
			Transition = 7
		}

		public enum MAP_PFN_TYPEEXTENDED {
			Unknown = 0,
			Unused = 1,
			ProcessPrivate = 2,
			PageTable = 3,
			LargePage = 4,
			DriverLocked = 5,
			Shareable = 6,
			File = 7
		}

		public enum MEMORYMODEL_TP {
			MEMORYMODEL_NA = 0,
			MEMORYMODEL_X86 = 1,
			MEMORYMODEL_X86PAE = 2,
			MEMORYMODEL_X64 = 3,
			MEMORYMODEL_ARM64 = 4
		}

		public enum SYSTEM_TP {
			SYSTEM_UNKNOWN_X64 = 1,
			SYSTEM_WINDOWS_X64 = 2,
			SYSTEM_UNKNOWN_X86 = 3,
			SYSTEM_WINDOWS_X86 = 4
		}
		//---------------------------------------------------------------------
		// CORE FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public const UInt64 OPT_CORE_PRINTF_ENABLE = 0x4000000100000000; // RW
		public const UInt64 OPT_CORE_VERBOSE = 0x4000000200000000; // RW
		public const UInt64 OPT_CORE_VERBOSE_EXTRA = 0x4000000300000000; // RW
		public const UInt64 OPT_CORE_VERBOSE_EXTRA_TLP = 0x4000000400000000; // RW
		public const UInt64 OPT_CORE_MAX_NATIVE_ADDRESS = 0x4000000800000000; // R

		public const UInt64
			OPT_CORE_LEECHCORE_HANDLE = 0x4000001000000000; // R - underlying leechcore handle (do not close).

		public const UInt64 OPT_CORE_SYSTEM = 0x2000000100000000; // R
		public const UInt64 OPT_CORE_MEMORYMODEL = 0x2000000200000000; // R

		public const UInt64 OPT_CONFIG_IS_REFRESH_ENABLED = 0x2000000300000000; // R - 1/0
		public const UInt64 OPT_CONFIG_TICK_PERIOD = 0x2000000400000000; // RW - base tick period in ms

		public const UInt64
			OPT_CONFIG_READCACHE_TICKS = 0x2000000500000000; // RW - memory cache validity period (in ticks)

		public const UInt64
			OPT_CONFIG_TLBCACHE_TICKS = 0x2000000600000000; // RW - page table (tlb) cache validity period (in ticks)

		public const UInt64
			OPT_CONFIG_PROCCACHE_TICKS_PARTIAL = 0x2000000700000000; // RW - process refresh (partial) period (in ticks)

		public const UInt64
			OPT_CONFIG_PROCCACHE_TICKS_TOTAL = 0x2000000800000000; // RW - process refresh (full) period (in ticks)

		public const UInt64 OPT_CONFIG_VMM_VERSION_MAJOR = 0x2000000900000000; // R
		public const UInt64 OPT_CONFIG_VMM_VERSION_MINOR = 0x2000000A00000000; // R
		public const UInt64 OPT_CONFIG_VMM_VERSION_REVISION = 0x2000000B00000000; // R

		public const UInt64
			OPT_CONFIG_STATISTICS_FUNCTIONCALL =
				0x2000000C00000000; // RW - enable function call statistics (.status/statistics_fncall file)

		public const UInt64 OPT_CONFIG_IS_PAGING_ENABLED = 0x2000000D00000000; // RW - 1/0
		public const UInt64 OPT_CONFIG_DEBUG = 0x2000000E00000000; // W
		public const UInt64 OPT_CONFIG_YARA_RULES = 0x2000000F00000000; // R

		public const UInt64 OPT_WIN_VERSION_MAJOR = 0x2000010100000000; // R
		public const UInt64 OPT_WIN_VERSION_MINOR = 0x2000010200000000; // R
		public const UInt64 OPT_WIN_VERSION_BUILD = 0x2000010300000000; // R
		public const UInt64 OPT_WIN_SYSTEM_UNIQUE_ID = 0x2000010400000000; // R

		public const UInt64 OPT_FORENSIC_MODE = 0x2000020100000000; // RW - enable/retrieve forensic mode type [0-4].

		// REFRESH OPTIONS:
		public const UInt64 OPT_REFRESH_ALL = 0x2001ffff00000000; // W - refresh all caches
		public const UInt64 OPT_REFRESH_FREQ_MEM = 0x2001100000000000; // W - refresh memory cache (excl. TLB) [fully]

		public const UInt64
			OPT_REFRESH_FREQ_MEM_PARTIAL =
				0x2001000200000000; // W - refresh memory cache (excl. TLB) [partial 33%/call]

		public const UInt64 OPT_REFRESH_FREQ_TLB = 0x2001080000000000; // W - refresh page table (TLB) cache [fully]

		public const UInt64
			OPT_REFRESH_FREQ_TLB_PARTIAL = 0x2001000400000000; // W - refresh page table (TLB) cache [partial 33%/call]

		public const UInt64
			OPT_REFRESH_FREQ_FAST = 0x2001040000000000; // W - refresh fast frequency - incl. partial process refresh

		public const UInt64
			OPT_REFRESH_FREQ_MEDIUM = 0x2001000100000000; // W - refresh medium frequency - incl. full process refresh

		public const UInt64 OPT_REFRESH_FREQ_SLOW = 0x2001001000000000; // W - refresh slow frequency.

		// PROCESS OPTIONS: [LO-DWORD: Process PID]
		public const UInt64 OPT_PROCESS_DTB = 0x2002000100000000; // W - force set process directory table base.


		//---------------------------------------------------------------------
		// MEMORY READ/WRITE FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public const UInt32
			PID_PROCESS_WITH_KERNELMEMORY =
				0x80000000; // Combine with dwPID to enable process kernel memory (NB! use with extreme care).

		public const UInt32
			FLAG_NOCACHE = 0x0001; // do not use the data cache (force reading from memory acquisition device)

		public const UInt32
			FLAG_ZEROPAD_ON_FAIL =
				0x0002; // zero pad failed physical memory reads and report success if read within range of physical memory.

		public const UInt32
			FLAG_FORCECACHE_READ =
				0x0008; // force use of cache - fail non-cached pages - only valid for reads, invalid with VMM_FLAG_NOCACHE/VMM_FLAG_ZEROPAD_ON_FAIL.

		public const UInt32
			FLAG_NOPAGING =
				0x0010; // do not try to retrieve memory from paged out memory from pagefile/compressed (even if possible)

		public const UInt32
			FLAG_NOPAGING_IO =
				0x0020; // do not try to retrieve memory from paged out memory if read would incur additional I/O (even if possible).

		public const UInt32
			FLAG_NOCACHEPUT =
				0x0100; // do not write back to the data cache upon successful read from memory acquisition device.

		public const UInt32
			FLAG_CACHE_RECENT_ONLY = 0x0200; // only fetch from the most recent active cache region when reading.

		public const UInt32 VMMDLL_PROCESS_INFORMATION_OPT_STRING_PATH_KERNEL = 1;
		public const UInt32 VMMDLL_PROCESS_INFORMATION_OPT_STRING_PATH_USER_IMAGE = 2;
		public const UInt32 VMMDLL_PROCESS_INFORMATION_OPT_STRING_CMDLINE = 3;


		//---------------------------------------------------------------------
		// "MAP" FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public const UInt64 MEMMAP_FLAG_PAGE_W = 0x0000000000000002;
		public const UInt64 MEMMAP_FLAG_PAGE_NS = 0x0000000000000004;
		public const UInt64 MEMMAP_FLAG_PAGE_NX = 0x8000000000000000;
		public const UInt64 MEMMAP_FLAG_PAGE_MASK = 0x8000000000000006;

		public const UInt32 MAP_MODULEENTRY_TP_NORMAL = 0;
		public const UInt32 VMMDLL_MODULE_TP_DATA = 1;
		public const UInt32 VMMDLL_MODULE_TP_NOTLINKED = 2;
		public const UInt32 VMMDLL_MODULE_TP_INJECTED = 3;

		private Boolean disposed;
		private IntPtr hVMM = IntPtr.Zero;

		// private zero-argument constructor - do not use!
		private Vmm() {
		}

		public Vmm(out LeechCore.LC_CONFIG_ERRORINFO ConfigErrorInfo, params String[] args) {
			this.hVMM = Initialize(out ConfigErrorInfo, args);
		}

		public Vmm(params String[] args) {
			LeechCore.LC_CONFIG_ERRORINFO ErrorInfo;
			this.hVMM = Initialize(out ErrorInfo, args);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static IntPtr Initialize(out LeechCore.LC_CONFIG_ERRORINFO ConfigErrorInfo, params String[] args) {
			IntPtr pLcErrorInfo;
			Int32 cbERROR_INFO = Marshal.SizeOf<lci.LC_CONFIG_ERRORINFO>();
			IntPtr hVMM = vmmi.VMMDLL_InitializeEx(args.Length, args, out pLcErrorInfo);
			Int64 vaLcCreateErrorInfo = pLcErrorInfo.ToInt64();
			ConfigErrorInfo = new LeechCore.LC_CONFIG_ERRORINFO();
			ConfigErrorInfo.strUserText = "";
			if (hVMM.ToInt64() == 0) {
				throw new Exception("VMM INIT FAILED.");
			}

			if (vaLcCreateErrorInfo == 0) {
				return hVMM;
			}

			lci.LC_CONFIG_ERRORINFO e = Marshal.PtrToStructure<lci.LC_CONFIG_ERRORINFO>(pLcErrorInfo);
			if (e.dwVersion == LeechCore.LC_CONFIG_ERRORINFO_VERSION) {
				ConfigErrorInfo.fValid = true;
				ConfigErrorInfo.fUserInputRequest = e.fUserInputRequest;
				if (e.cwszUserText > 0) {
					ConfigErrorInfo.strUserText = Marshal.PtrToStringUni((IntPtr)(vaLcCreateErrorInfo + cbERROR_INFO));
				}
			}

			lci.LcMemFree(pLcErrorInfo);
			return hVMM;
		}

		~Vmm() {
			this.Dispose(false);
		}

		private void Dispose(Boolean disposing) {
			if (!this.disposed) {
				vmmi.VMMDLL_Close(this.hVMM);
				this.hVMM = IntPtr.Zero;
				this.disposed = true;
			}
		}

		public void Close() {
			this.Dispose(true);
		}

		public static void CloseAll() {
			vmmi.VMMDLL_CloseAll();
		}

		public Boolean ConfigGet(UInt64 fOption, out UInt64 pqwValue) {
			return vmmi.VMMDLL_ConfigGet(this.hVMM, fOption, out pqwValue);
		}

		public Boolean ConfigSet(UInt64 fOption, UInt64 qwValue) {
			return vmmi.VMMDLL_ConfigSet(this.hVMM, fOption, qwValue);
		}

		public String GetMemoryMap() {
			MAP_PHYSMEMENTRY[] map = this.Map_GetPhysMem();
			if (map.Length == 0) {
				return null;
			}

			StringBuilder sb = new StringBuilder();
			Int32 leftLength = map.Max(x => x.pa).ToString("x").Length;
			for (Int32 i = 0; i < map.Length; i++) {
				sb.AppendFormat($"{{0,{-leftLength}}}", map[i].pa.ToString("x"))
					.Append($" - {(map[i].pa + map[i].cb - 1).ToString("x")}")
					.AppendLine();
			}

			return sb.ToString();
		}

		public Boolean VfsList(String wszPath, UInt64 h, VfsCallBack_AddFile CallbackFile,
			VfsCallBack_AddDirectory CallbackDirectory) {
			vmmi.VMMDLL_VFS_FILELIST FileList;
			FileList.dwVersion = vmmi.VMMDLL_VFS_FILELIST_VERSION;
			FileList.h = h;
			FileList._Reserved = 0;
			FileList.pfnAddFile = Marshal.GetFunctionPointerForDelegate(CallbackFile);
			FileList.pfnAddDirectory = Marshal.GetFunctionPointerForDelegate(CallbackDirectory);
			return vmmi.VMMDLL_VfsList(this.hVMM, wszPath, ref FileList);
		}

		public unsafe UInt32 VfsRead(String wszFileName, UInt32 cb, UInt64 cbOffset, out Byte[] pbData) {
			UInt32 nt, cbRead = 0;
			Byte[] data = new Byte[cb];
			fixed (Byte* pb = data) {
				nt = vmmi.VMMDLL_VfsRead(this.hVMM, wszFileName, pb, cb, out cbRead, cbOffset);
				pbData = new Byte[cbRead];
				if (cbRead > 0) {
					Buffer.BlockCopy(data, 0, pbData, 0, (Int32)cbRead);
				}

				return nt;
			}
		}

		public unsafe UInt32 VfsWrite(String wszFileName, Byte[] pbData, UInt64 cbOffset) {
			UInt32 cbRead = 0;
			fixed (Byte* pb = pbData) {
				return vmmi.VMMDLL_VfsWrite(this.hVMM, wszFileName, pb, (UInt32)pbData.Length, out cbRead, cbOffset);
			}
		}


		//---------------------------------------------------------------------
		// PLUGIN FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public Boolean InitializePlugins() {
			return vmmi.VMMDLL_InitializePlugins(this.hVMM);
		}

		public MEM_SCATTER[] MemReadScatter(UInt32 pid, UInt32 flags, params UInt64[] qwA) {
			Int32 i;
			Int64 vappMEMs, vapMEM;
			IntPtr pMEM, pMEM_qwA, pppMEMs;
			if (!lci.LcAllocScatter1((UInt32)qwA.Length, out pppMEMs)) {
				return null;
			}

			vappMEMs = pppMEMs.ToInt64();
			for (i = 0; i < qwA.Length; i++) {
				vapMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8)).ToInt64();
				pMEM_qwA = new IntPtr(vapMEM + 8);
				Marshal.WriteInt64(pMEM_qwA, (Int64)(qwA[i] & ~(UInt64)0xfff));
			}

			MEM_SCATTER[] MEMs = new MEM_SCATTER[qwA.Length];
			vmmi.VMMDLL_MemReadScatter(this.hVMM, pid, pppMEMs, (UInt32)MEMs.Length, flags);
			for (i = 0; i < MEMs.Length; i++) {
				pMEM = Marshal.ReadIntPtr(new IntPtr(vappMEMs + i * 8));
				lci.LC_MEM_SCATTER n = Marshal.PtrToStructure<lci.LC_MEM_SCATTER>(pMEM);
				MEMs[i].f = n.f;
				MEMs[i].qwA = n.qwA;
				MEMs[i].pb = new Byte[0x1000];
				Marshal.Copy(n.pb, MEMs[i].pb, 0, 0x1000);
			}

			lci.LcMemFree(pppMEMs);
			return MEMs;
		}

		public VmmScatter Scatter_Initialize(UInt32 pid, UInt32 flags) {
			IntPtr hS = vmmi.VMMDLL_Scatter_Initialize(this.hVMM, pid, flags);
			if (hS.ToInt64() == 0) {
				return null;
			}

			return new VmmScatter(hS);
		}

		public unsafe Byte[] MemRead(UInt32 pid, UInt64 qwA, UInt32 cb, UInt32 flags = 0) {
			UInt32 cbRead;
			Byte[] data = new Byte[cb];
			fixed (Byte* pb = data) {
				if (!vmmi.VMMDLL_MemReadEx(this.hVMM, pid, qwA, pb, cb, out cbRead, flags)) {
					return null;
				}
			}

			if (cbRead != cb) {
				Array.Resize(ref data, (Int32)cbRead);
			}

			return data;
		}

		public unsafe Boolean MemReadStruct<T>(UInt32 pid, UInt64 qwA, out T result, UInt32 flags = 0)
			where T : unmanaged {
			UInt32 cb = (UInt32)sizeof(T);
			result = default;
			UInt32 cbRead;
			fixed (T* pb = &result) {
				if (!vmmi.VMMDLL_MemReadEx(this.hVMM, pid, qwA, (Byte*)pb, cb, out cbRead, flags)) {
					return false;
				}
			}

			if (cbRead != cb) {
				return false;
			}

			return true;
		}

		public unsafe Boolean MemPrefetchPages(UInt32 pid, UInt64[] qwA) {
			Byte[] data = new Byte[qwA.Length * sizeof(UInt64)];
			Buffer.BlockCopy(qwA, 0, data, 0, data.Length);
			fixed (Byte* pb = data) {
				return vmmi.VMMDLL_MemPrefetchPages(this.hVMM, pid, pb, (UInt32)qwA.Length);
			}
		}

		public unsafe Boolean MemWrite(UInt32 pid, UInt64 qwA, Byte[] data) {
			fixed (Byte* pb = data) {
				return vmmi.VMMDLL_MemWrite(this.hVMM, pid, qwA, pb, (UInt32)data.Length);
			}
		}

		public unsafe Boolean MemWriteStruct<T>(UInt32 pid, UInt64 qwA, T value)
			where T : unmanaged {
			UInt32 cb = (UInt32)sizeof(T);
			Byte* pb = (Byte*)&value;
			return vmmi.VMMDLL_MemWrite(this.hVMM, pid, qwA, pb, cb);
		}

		public Boolean MemVirt2Phys(UInt32 dwPID, UInt64 qwVA, out UInt64 pqwPA) {
			return vmmi.VMMDLL_MemVirt2Phys(this.hVMM, dwPID, qwVA, out pqwPA);
		}

		public unsafe UInt64[] MemSearchM(UInt32 pid, VMMDLL_MEM_SEARCHENTRY[] search, UInt64 vaMin = 0,
			UInt64 vaMax = 0xffffffffffffffff, UInt32 cMaxResult = 0x10000, UInt32 ReadFlags = 0) {
			// checks:
			if (search == null || search.Length == 0 || search.Length > 16) {
				return new UInt64[0];
			}

			// check search items and convert:
			vmmi.VMMDLL_MEM_SEARCH_CONTEXT_SEARCHENTRY[] es = new vmmi.VMMDLL_MEM_SEARCH_CONTEXT_SEARCHENTRY[16];
			for (Int32 i = 0; i < search.Length; i++) {
				if (search[i].pbSearch == null || search[i].pbSearch.Length == 0 || search[i].pbSearch.Length > 32) {
					return new UInt64[0];
				}

				if (search[i].pbSearchSkipMask != null &&
					search[i].pbSearchSkipMask.Length > search[i].pbSearch.Length) {
					return new UInt64[0];
				}

				es[i].cbAlign = search[i].cbAlign;
				es[i].cb = (UInt32)search[i].pbSearch.Length;
				es[i].pb = new Byte[32];
				search[i].pbSearch.CopyTo(es[i].pb, 0);
				if (search[i].pbSearchSkipMask != null && search[i].pbSearchSkipMask.Length > 0) {
					es[i].pbSkipMask = new Byte[32];
					search[i].pbSearchSkipMask.CopyTo(es[i].pbSkipMask, 0);
				}
			}

			// initialize search struct:
			vmmi.VMMDLL_MEM_SEARCH_CONTEXT ctx = new vmmi.VMMDLL_MEM_SEARCH_CONTEXT();
			ctx.dwVersion = vmmi.VMMDLL_MEM_SEARCH_VERSION;
			ctx.cMaxResult = cMaxResult;
			ctx.cSearch = 1;
			ctx.vaMin = vaMin;
			ctx.vaMax = vaMax;
			ctx.ReadFlags = ReadFlags;
			ctx.search = es;
			// perform native search:
			UInt32 pcva;
			IntPtr ppva;
			if (!vmmi.VMMDLL_MemSearch(this.hVMM, pid, ref ctx, out ppva, out pcva)) {
				return new UInt64[0];
			}

			UInt64[] result = new UInt64[pcva];
			for (Int32 i = 0; i < pcva; i++) {
				result[i] = Marshal.PtrToStructure<UInt64>(IntPtr.Add(ppva, i * 8));
			}

			vmmi.VMMDLL_MemFree((Byte*)ppva.ToPointer());
			return result;
		}

		public UInt64[] MemSearch1(UInt32 pid, Byte[] pbSearch, UInt64 vaMin = 0, UInt64 vaMax = 0xffffffffffffffff,
			UInt32 cMaxResult = 0x10000, UInt32 ReadFlags = 0, Byte[] pbSearchSkipMask = null, UInt32 cbAlign = 1) {
			VMMDLL_MEM_SEARCHENTRY[] es = new VMMDLL_MEM_SEARCHENTRY[1];
			es[0].cbAlign = cbAlign;
			es[0].pbSearch = pbSearch;
			es[0].pbSearchSkipMask = pbSearchSkipMask;
			return this.MemSearchM(pid, es, vaMin, vaMax, cMaxResult, ReadFlags);
		}

		public Boolean PidGetFromName(String szProcName, out UInt32 pdwPID) {
			return vmmi.VMMDLL_PidGetFromName(this.hVMM, szProcName, out pdwPID);
		}

		public unsafe UInt32[] PidList() {
			Boolean result;
			UInt64 c = 0;
			result = vmmi.VMMDLL_PidList(this.hVMM, null, ref c);
			if (!result || c == 0) {
				return new UInt32[0];
			}

			fixed (Byte* pb = new Byte[c * 4]) {
				result = vmmi.VMMDLL_PidList(this.hVMM, pb, ref c);
				if (!result || c == 0) {
					return new UInt32[0];
				}

				UInt32[] m = new UInt32[c];
				for (UInt64 i = 0; i < c; i++) {
					m[i] = (UInt32)Marshal.ReadInt32((IntPtr)(pb + i * 4));
				}

				return m;
			}
		}

		public unsafe PROCESS_INFORMATION ProcessGetInformation(UInt32 pid) {
			Boolean result;
			UInt64 cbENTRY = (UInt64)Marshal.SizeOf<vmmi.VMMDLL_PROCESS_INFORMATION>();
			fixed (Byte* pb = new Byte[cbENTRY]) {
				Marshal.WriteInt64(new IntPtr(pb + 0), unchecked((Int64)vmmi.VMMDLL_PROCESS_INFORMATION_MAGIC));
				Marshal.WriteInt16(new IntPtr(pb + 8), unchecked((Int16)vmmi.VMMDLL_PROCESS_INFORMATION_VERSION));
				result = vmmi.VMMDLL_ProcessGetInformation(this.hVMM, pid, pb, ref cbENTRY);
				if (!result) {
					return new PROCESS_INFORMATION();
				}

				vmmi.VMMDLL_PROCESS_INFORMATION n = Marshal.PtrToStructure<vmmi.VMMDLL_PROCESS_INFORMATION>((IntPtr)pb);
				if (n.wVersion != vmmi.VMMDLL_PROCESS_INFORMATION_VERSION) {
					return new PROCESS_INFORMATION();
				}

				PROCESS_INFORMATION e;
				e.fValid = true;
				e.tpMemoryModel = n.tpMemoryModel;
				e.tpSystem = n.tpSystem;
				e.fUserOnly = n.fUserOnly;
				e.dwPID = n.dwPID;
				e.dwPPID = n.dwPPID;
				e.dwState = n.dwState;
				e.szName = n.szName;
				e.szNameLong = n.szNameLong;
				e.paDTB = n.paDTB;
				e.paDTB_UserOpt = n.paDTB_UserOpt;
				e.vaEPROCESS = n.vaEPROCESS;
				e.vaPEB = n.vaPEB;
				e.fWow64 = n.fWow64;
				e.vaPEB32 = n.vaPEB32;
				e.dwSessionId = n.dwSessionId;
				e.qwLUID = n.qwLUID;
				e.szSID = n.szSID;
				e.IntegrityLevel = n.IntegrityLevel;
				return e;
			}
		}

		public unsafe String ProcessGetInformationString(UInt32 pid, UInt32 fOptionString) {
			Byte* pb = vmmi.VMMDLL_ProcessGetInformationString(this.hVMM, pid, fOptionString);
			if (pb == null) {
				return "";
			}

			String s = Marshal.PtrToStringAnsi((IntPtr)pb);
			vmmi.VMMDLL_MemFree(pb);
			return s;
		}

		public unsafe IMAGE_DATA_DIRECTORY[] ProcessGetDirectories(UInt32 pid, String wszModule) {
			String[] PE_DATA_DIRECTORIES = new String[16] {
				"EXPORT", "IMPORT", "RESOURCE", "EXCEPTION", "SECURITY", "BASERELOC", "DEBUG", "ARCHITECTURE",
				"GLOBALPTR", "TLS", "LOAD_CONFIG", "BOUND_IMPORT", "IAT", "DELAY_IMPORT", "COM_DESCRIPTOR",
				"RESERVED"
			};
			Boolean result;
			UInt32 cbENTRY = (UInt32)Marshal.SizeOf<vmmi.VMMDLL_IMAGE_DATA_DIRECTORY>();
			fixed (Byte* pb = new Byte[16 * cbENTRY]) {
				result = vmmi.VMMDLL_ProcessGetDirectories(this.hVMM, pid, wszModule, pb);
				if (!result) {
					return new IMAGE_DATA_DIRECTORY[0];
				}

				IMAGE_DATA_DIRECTORY[] m = new IMAGE_DATA_DIRECTORY[16];
				for (Int32 i = 0; i < 16; i++) {
					vmmi.VMMDLL_IMAGE_DATA_DIRECTORY n =
						Marshal.PtrToStructure<vmmi.VMMDLL_IMAGE_DATA_DIRECTORY>((IntPtr)(pb + i * cbENTRY));
					IMAGE_DATA_DIRECTORY e;
					e.name = PE_DATA_DIRECTORIES[i];
					e.VirtualAddress = n.VirtualAddress;
					e.Size = n.Size;
					m[i] = e;
				}

				return m;
			}
		}

		public unsafe IMAGE_SECTION_HEADER[] ProcessGetSections(UInt32 pid, String wszModule) {
			Boolean result;
			UInt32 cData;
			UInt32 cbENTRY = (UInt32)Marshal.SizeOf<vmmi.VMMDLL_IMAGE_SECTION_HEADER>();
			result = vmmi.VMMDLL_ProcessGetSections(this.hVMM, pid, wszModule, null, 0, out cData);
			if (!result || cData == 0) {
				return new IMAGE_SECTION_HEADER[0];
			}

			fixed (Byte* pb = new Byte[cData * cbENTRY]) {
				result = vmmi.VMMDLL_ProcessGetSections(this.hVMM, pid, wszModule, pb, cData, out cData);
				if (!result || cData == 0) {
					return new IMAGE_SECTION_HEADER[0];
				}

				IMAGE_SECTION_HEADER[] m = new IMAGE_SECTION_HEADER[cData];
				for (Int32 i = 0; i < cData; i++) {
					vmmi.VMMDLL_IMAGE_SECTION_HEADER n =
						Marshal.PtrToStructure<vmmi.VMMDLL_IMAGE_SECTION_HEADER>((IntPtr)(pb + i * cbENTRY));
					IMAGE_SECTION_HEADER e;
					e.Name = n.Name;
					e.MiscPhysicalAddressOrVirtualSize = n.MiscPhysicalAddressOrVirtualSize;
					e.VirtualAddress = n.VirtualAddress;
					e.SizeOfRawData = n.SizeOfRawData;
					e.PointerToRawData = n.PointerToRawData;
					e.PointerToRelocations = n.PointerToRelocations;
					e.PointerToLinenumbers = n.PointerToLinenumbers;
					e.NumberOfRelocations = n.NumberOfRelocations;
					e.NumberOfLinenumbers = n.NumberOfLinenumbers;
					e.Characteristics = n.Characteristics;
					m[i] = e;
				}

				return m;
			}
		}

		public UInt64 ProcessGetProcAddress(UInt32 pid, String wszModuleName, String szFunctionName) {
			return vmmi.VMMDLL_ProcessGetProcAddress(this.hVMM, pid, wszModuleName, szFunctionName);
		}

		public UInt64 ProcessGetModuleBase(UInt32 pid, String wszModuleName) {
			return vmmi.VMMDLL_ProcessGetModuleBase(this.hVMM, pid, wszModuleName);
		}


		//---------------------------------------------------------------------
		// WINDOWS SPECIFIC DEBUGGING / SYMBOL FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public unsafe Boolean PdbLoad(UInt32 pid, UInt64 vaModuleBase, out String szModuleName) {
			szModuleName = "";
			Byte[] data = new Byte[260];
			fixed (Byte* pb = data) {
				Boolean result = vmmi.VMMDLL_PdbLoad(this.hVMM, pid, vaModuleBase, pb);
				if (!result) {
					return false;
				}

				szModuleName = Encoding.UTF8.GetString(data);
				szModuleName = szModuleName.Substring(0, szModuleName.IndexOf((Char)0));
			}

			return true;
		}

		public unsafe Boolean PdbSymbolName(String szModule, UInt64 cbSymbolAddressOrOffset, out String szSymbolName,
			out UInt32 pdwSymbolDisplacement) {
			szSymbolName = "";
			pdwSymbolDisplacement = 0;
			Byte[] data = new Byte[260];
			fixed (Byte* pb = data) {
				Boolean result = vmmi.VMMDLL_PdbSymbolName(this.hVMM, szModule, cbSymbolAddressOrOffset, pb,
					out pdwSymbolDisplacement);
				if (!result) {
					return false;
				}

				szSymbolName = Encoding.UTF8.GetString(data);
				szSymbolName = szSymbolName.Substring(0, szSymbolName.IndexOf((Char)0));
			}

			return true;
		}

		public Boolean PdbSymbolAddress(String szModule, String szSymbolName, out UInt64 pvaSymbolAddress) {
			return vmmi.VMMDLL_PdbSymbolAddress(this.hVMM, szModule, szSymbolName, out pvaSymbolAddress);
		}

		public Boolean PdbTypeSize(String szModule, String szTypeName, out UInt32 pcbTypeSize) {
			return vmmi.VMMDLL_PdbTypeSize(this.hVMM, szModule, szTypeName, out pcbTypeSize);
		}

		public Boolean PdbTypeChildOffset(String szModule, String szTypeName, String wszTypeChildName,
			out UInt32 pcbTypeChildOffset) {
			return vmmi.VMMDLL_PdbTypeChildOffset(this.hVMM, szModule, szTypeName, wszTypeChildName,
				out pcbTypeChildOffset);
		}

		public unsafe MAP_PTEENTRY[] Map_GetPte(UInt32 pid, Boolean fIdentifyModules = true) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_PTE>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_PTEENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_PTEENTRY[] m = new MAP_PTEENTRY[0];
			if (!vmmi.VMMDLL_Map_GetPte(this.hVMM, pid, fIdentifyModules, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_PTE nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PTE>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_PTE_VERSION) {
				goto fail;
			}

			m = new MAP_PTEENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_PTEENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PTEENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_PTEENTRY e;
				e.vaBase = n.vaBase;
				e.vaEnd = n.vaBase + (n.cPages << 12) - 1;
				e.cbSize = n.cPages << 12;
				e.cPages = n.cPages;
				e.fPage = n.fPage;
				e.fWoW64 = n.fWoW64;
				e.wszText = n.wszText;
				e.cSoftware = n.cSoftware;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_VADENTRY[] Map_GetVad(UInt32 pid, Boolean fIdentifyModules = true) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_VAD>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_VADENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_VADENTRY[] m = new MAP_VADENTRY[0];
			if (!vmmi.VMMDLL_Map_GetVad(this.hVMM, pid, fIdentifyModules, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_VAD nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_VAD>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_VAD_VERSION) {
				goto fail;
			}

			m = new MAP_VADENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_VADENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_VADENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_VADENTRY e;
				e.vaStart = n.vaStart;
				e.vaEnd = n.vaEnd;
				e.cbSize = n.vaEnd + 1 - n.vaStart;
				e.vaVad = n.vaVad;
				e.VadType = n.dw0 & 0x07;
				e.Protection = (n.dw0 >> 3) & 0x1f;
				e.fImage = ((n.dw0 >> 8) & 1) == 1;
				e.fFile = ((n.dw0 >> 9) & 1) == 1;
				e.fPageFile = ((n.dw0 >> 10) & 1) == 1;
				e.fPrivateMemory = ((n.dw0 >> 11) & 1) == 1;
				e.fTeb = ((n.dw0 >> 12) & 1) == 1;
				e.fStack = ((n.dw0 >> 13) & 1) == 1;
				e.fSpare = (n.dw0 >> 14) & 0x03;
				e.HeapNum = (n.dw0 >> 16) & 0x1f;
				e.fHeap = ((n.dw0 >> 23) & 1) == 1;
				e.cwszDescription = (n.dw0 >> 24) & 0xff;
				e.CommitCharge = n.dw1 & 0x7fffffff;
				e.MemCommit = ((n.dw1 >> 31) & 1) == 1;
				e.u2 = n.u2;
				e.cbPrototypePte = n.cbPrototypePte;
				e.vaPrototypePte = n.vaPrototypePte;
				e.vaSubsection = n.vaSubsection;
				e.wszText = n.wszText;
				e.vaFileObject = n.vaFileObject;
				e.cVadExPages = n.cVadExPages;
				e.cVadExPagesBase = n.cVadExPagesBase;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_VADEXENTRY[] Map_GetVadEx(UInt32 pid, UInt32 oPages, UInt32 cPages) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_VADEX>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_VADEXENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_VADEXENTRY[] m = new MAP_VADEXENTRY[0];
			if (!vmmi.VMMDLL_Map_GetVadEx(this.hVMM, pid, oPages, cPages, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_VADEX nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_VADEX>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_VADEX_VERSION) {
				goto fail;
			}

			m = new MAP_VADEXENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_VADEXENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_VADEXENTRY>((IntPtr)(pMap.ToInt64() + cbMAP +
						i * cbENTRY));
				MAP_VADEXENTRY e;
				e.tp = n.tp;
				e.iPML = n.iPML;
				e.pteFlags = n.pteFlags;
				e.va = n.va;
				e.pa = n.pa;
				e.pte = n.pte;
				e.proto.tp = n.proto_tp;
				e.proto.pa = n.proto_pa;
				e.proto.pte = n.proto_pte;
				e.vaVadBase = n.vaVadBase;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_MODULEENTRY[] Map_GetModule(UInt32 pid, Boolean fExtendedInfo) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_MODULE>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_MODULEENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_MODULEENTRY[] m = new MAP_MODULEENTRY[0];
			UInt32 flags = fExtendedInfo ? (UInt32)0xff : 0;
			if (!vmmi.VMMDLL_Map_GetModule(this.hVMM, pid, out pMap, flags)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_MODULE nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_MODULE>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_MODULE_VERSION) {
				goto fail;
			}

			m = new MAP_MODULEENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_MODULEENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_MODULEENTRY>(
						(IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_MODULEENTRY e;
				MODULEENTRY_DEBUGINFO eDbg;
				MODULEENTRY_VERSIONINFO eVer;
				e.fValid = true;
				e.vaBase = n.vaBase;
				e.vaEntry = n.vaEntry;
				e.cbImageSize = n.cbImageSize;
				e.fWow64 = n.fWow64;
				e.wszText = n.wszText;
				e.wszFullName = n.wszFullName;
				e.tp = n.tp;
				e.cbFileSizeRaw = n.cbFileSizeRaw;
				e.cSection = n.cSection;
				e.cEAT = n.cEAT;
				e.cIAT = n.cIAT;
				// Extended Debug Information:
				if (n.pExDebugInfo.ToInt64() == 0) {
					eDbg.fValid = false;
					eDbg.dwAge = 0;
					eDbg.wszGuid = "";
					eDbg.wszPdbFilename = "";
				}
				else {
					vmmi.VMMDLL_MAP_MODULEENTRY_DEBUGINFO nDbg =
						Marshal.PtrToStructure<vmmi.VMMDLL_MAP_MODULEENTRY_DEBUGINFO>(n.pExDebugInfo);
					eDbg.fValid = true;
					eDbg.dwAge = nDbg.dwAge;
					eDbg.wszGuid = nDbg.wszGuid;
					eDbg.wszPdbFilename = nDbg.wszPdbFilename;
				}

				e.DebugInfo = eDbg;
				// Extended Version Information
				if (n.pExDebugInfo.ToInt64() == 0) {
					eVer.fValid = false;
					eVer.wszCompanyName = "";
					eVer.wszFileDescription = "";
					eVer.wszFileVersion = "";
					eVer.wszInternalName = "";
					eVer.wszLegalCopyright = "";
					eVer.wszFileOriginalFilename = "";
					eVer.wszProductName = "";
					eVer.wszProductVersion = "";
				}
				else {
					vmmi.VMMDLL_MAP_MODULEENTRY_VERSIONINFO nVer =
						Marshal.PtrToStructure<vmmi.VMMDLL_MAP_MODULEENTRY_VERSIONINFO>(n.pExVersionInfo);
					eVer.fValid = true;
					eVer.wszCompanyName = nVer.wszCompanyName;
					eVer.wszFileDescription = nVer.wszFileDescription;
					eVer.wszFileVersion = nVer.wszFileVersion;
					eVer.wszInternalName = nVer.wszInternalName;
					eVer.wszLegalCopyright = nVer.wszLegalCopyright;
					eVer.wszFileOriginalFilename = nVer.wszFileOriginalFilename;
					eVer.wszProductName = nVer.wszProductName;
					eVer.wszProductVersion = nVer.wszProductVersion;
				}

				e.VersionInfo = eVer;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_MODULEENTRY Map_GetModuleFromName(UInt32 pid, String wszModuleName) {
			IntPtr pMap = IntPtr.Zero;
			MAP_MODULEENTRY e = new MAP_MODULEENTRY();
			if (!vmmi.VMMDLL_Map_GetModuleFromName(this.hVMM, pid, wszModuleName, out pMap, 0)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_MODULEENTRY nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_MODULEENTRY>(pMap);
			e.fValid = true;
			e.vaBase = nM.vaBase;
			e.vaEntry = nM.vaEntry;
			e.cbImageSize = nM.cbImageSize;
			e.fWow64 = nM.fWow64;
			e.wszText = wszModuleName;
			e.wszFullName = nM.wszFullName;
			e.tp = nM.tp;
			e.cbFileSizeRaw = nM.cbFileSizeRaw;
			e.cSection = nM.cSection;
			e.cEAT = nM.cEAT;
			e.cIAT = nM.cIAT;
		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return e;
		}

		public unsafe MAP_UNLOADEDMODULEENTRY[] Map_GetUnloadedModule(UInt32 pid) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_UNLOADEDMODULE>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_UNLOADEDMODULEENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_UNLOADEDMODULEENTRY[] m = new MAP_UNLOADEDMODULEENTRY[0];
			if (!vmmi.VMMDLL_Map_GetUnloadedModule(this.hVMM, pid, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_UNLOADEDMODULE nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_UNLOADEDMODULE>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_UNLOADEDMODULE_VERSION) {
				goto fail;
			}

			m = new MAP_UNLOADEDMODULEENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_UNLOADEDMODULEENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_UNLOADEDMODULEENTRY>(
						(IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_UNLOADEDMODULEENTRY e;
				e.vaBase = n.vaBase;
				e.cbImageSize = n.cbImageSize;
				e.fWow64 = n.fWow64;
				e.wszText = n.wszText;
				e.dwCheckSum = n.dwCheckSum;
				e.dwTimeDateStamp = n.dwTimeDateStamp;
				e.ftUnload = n.ftUnload;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_EATENTRY[] Map_GetEAT(UInt32 pid, String wszModule, out MAP_EATINFO EatInfo) {
			EatInfo = new MAP_EATINFO();
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_EAT>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_EATENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_EATENTRY[] m = new MAP_EATENTRY[0];
			if (!vmmi.VMMDLL_Map_GetEAT(this.hVMM, pid, wszModule, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_EAT nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_EAT>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_EAT_VERSION) {
				goto fail;
			}

			m = new MAP_EATENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_EATENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_EATENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_EATENTRY e;
				e.vaFunction = n.vaFunction;
				e.dwOrdinal = n.dwOrdinal;
				e.oFunctionsArray = n.oFunctionsArray;
				e.oNamesArray = n.oNamesArray;
				e.wszFunction = n.wszFunction;
				e.wszForwardedFunction = n.wszForwardedFunction;
				m[i] = e;
			}

			EatInfo.fValid = true;
			EatInfo.vaModuleBase = nM.vaModuleBase;
			EatInfo.vaAddressOfFunctions = nM.vaAddressOfFunctions;
			EatInfo.vaAddressOfNames = nM.vaAddressOfNames;
			EatInfo.cNumberOfFunctions = nM.cNumberOfFunctions;
			EatInfo.cNumberOfForwardedFunctions = nM.cNumberOfForwardedFunctions;
			EatInfo.cNumberOfNames = nM.cNumberOfNames;
			EatInfo.dwOrdinalBase = nM.dwOrdinalBase;
		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_IATENTRY[] Map_GetIAT(UInt32 pid, String wszModule) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_IAT>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_IATENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_IATENTRY[] m = new MAP_IATENTRY[0];
			if (!vmmi.VMMDLL_Map_GetIAT(this.hVMM, pid, wszModule, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_IAT nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_IAT>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_IAT_VERSION) {
				goto fail;
			}

			m = new MAP_IATENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_IATENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_IATENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_IATENTRY e;
				e.vaFunction = n.vaFunction;
				e.wszFunction = n.wszFunction;
				e.wszModule = n.wszModule;
				e.f32 = n.f32;
				e.wHint = n.wHint;
				e.rvaFirstThunk = n.rvaFirstThunk;
				e.rvaOriginalFirstThunk = n.rvaOriginalFirstThunk;
				e.rvaNameModule = n.rvaNameModule;
				e.rvaNameFunction = n.rvaNameFunction;
				e.vaModule = nM.vaModuleBase;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_HEAP Map_GetHeap(UInt32 pid) {
			IntPtr pMap = IntPtr.Zero;
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_HEAP>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_HEAPENTRY>();
			Int32 cbSEGENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_HEAPSEGMENTENTRY>();
			MAP_HEAP Heap;
			Heap.heaps = new MAP_HEAPENTRY[0];
			Heap.segments = new MAP_HEAPSEGMENTENTRY[0];
			if (!vmmi.VMMDLL_Map_GetHeap(this.hVMM, pid, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_HEAP nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HEAP>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_HEAP_VERSION) {
				goto fail;
			}

			Heap.heaps = new MAP_HEAPENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_HEAPENTRY nH =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HEAPENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				Heap.heaps[i].va = nH.va;
				Heap.heaps[i].f32 = nH.f32;
				Heap.heaps[i].tpHeap = nH.tp;
				Heap.heaps[i].iHeapNum = nH.dwHeapNum;
			}

			Heap.segments = new MAP_HEAPSEGMENTENTRY[nM.cSegments];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_HEAPSEGMENTENTRY nH =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HEAPSEGMENTENTRY>((IntPtr)(nM.pSegments.ToInt64() +
						i * cbSEGENTRY));
				Heap.segments[i].va = nH.va;
				Heap.segments[i].cb = nH.cb;
				Heap.segments[i].tpHeapSegment = nH.tp;
				Heap.segments[i].iHeapNum = nH.iHeap;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return Heap;
		}

		public unsafe MAP_HEAPALLOCENTRY[] Map_GetHeapAlloc(UInt32 pid, UInt64 vaHeapOrHeapNum) {
			IntPtr pHeapAllocMap = IntPtr.Zero;
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_HEAPALLOC>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_HEAPALLOCENTRY>();
			if (!vmmi.VMMDLL_Map_GetHeapAlloc(this.hVMM, pid, vaHeapOrHeapNum, out pHeapAllocMap)) {
				return new MAP_HEAPALLOCENTRY[0];
			}

			vmmi.VMMDLL_MAP_HEAPALLOC nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HEAPALLOC>(pHeapAllocMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_HEAPALLOC_VERSION) {
				vmmi.VMMDLL_MemFree((Byte*)pHeapAllocMap.ToPointer());
				return new MAP_HEAPALLOCENTRY[0];
			}

			MAP_HEAPALLOCENTRY[] m = new MAP_HEAPALLOCENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_HEAPALLOCENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HEAPALLOCENTRY>((IntPtr)(pHeapAllocMap.ToInt64() + cbMAP +
						i * cbENTRY));
				m[i].va = n.va;
				m[i].cb = n.cb;
				m[i].tp = n.tp;
			}

			vmmi.VMMDLL_MemFree((Byte*)pHeapAllocMap.ToPointer());
			return m;
		}

		public unsafe MAP_THREADENTRY[] Map_GetThread(UInt32 pid) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_THREAD>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_THREADENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_THREADENTRY[] m = new MAP_THREADENTRY[0];
			if (!vmmi.VMMDLL_Map_GetThread(this.hVMM, pid, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_THREAD nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_THREAD>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_THREAD_VERSION) {
				goto fail;
			}

			m = new MAP_THREADENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_THREADENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_THREADENTRY>(
						(IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_THREADENTRY e;
				e.dwTID = n.dwTID;
				e.dwPID = n.dwPID;
				e.dwExitStatus = n.dwExitStatus;
				e.bState = n.bState;
				e.bRunning = n.bRunning;
				e.bPriority = n.bPriority;
				e.bBasePriority = n.bBasePriority;
				e.vaETHREAD = n.vaETHREAD;
				e.vaTeb = n.vaTeb;
				e.ftCreateTime = n.ftCreateTime;
				e.ftExitTime = n.ftExitTime;
				e.vaStartAddress = n.vaStartAddress;
				e.vaWin32StartAddress = n.vaWin32StartAddress;
				e.vaStackBaseUser = n.vaStackBaseUser;
				e.vaStackLimitUser = n.vaStackLimitUser;
				e.vaStackBaseKernel = n.vaStackBaseKernel;
				e.vaStackLimitKernel = n.vaStackLimitKernel;
				e.vaImpersonationToken = n.vaImpersonationToken;
				e.vaTrapFrame = n.vaTrapFrame;
				e.vaRIP = n.vaRIP;
				e.vaRSP = n.vaRSP;
				e.qwAffinity = n.qwAffinity;
				e.dwUserTime = n.dwUserTime;
				e.dwKernelTime = n.dwKernelTime;
				e.bSuspendCount = n.bSuspendCount;
				e.bWaitReason = n.bWaitReason;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_HANDLEENTRY[] Map_GetHandle(UInt32 pid) {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_HANDLE>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_HANDLEENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_HANDLEENTRY[] m = new MAP_HANDLEENTRY[0];
			if (!vmmi.VMMDLL_Map_GetHandle(this.hVMM, pid, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_HANDLE nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HANDLE>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_HANDLE_VERSION) {
				goto fail;
			}

			m = new MAP_HANDLEENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_HANDLEENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_HANDLEENTRY>(
						(IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_HANDLEENTRY e;
				e.vaObject = n.vaObject;
				e.dwHandle = n.dwHandle;
				e.dwGrantedAccess = n.dwGrantedAccess_iType & 0x00ffffff;
				e.iType = n.dwGrantedAccess_iType >> 24;
				e.qwHandleCount = n.qwHandleCount;
				e.qwPointerCount = n.qwPointerCount;
				e.vaObjectCreateInfo = n.vaObjectCreateInfo;
				e.vaSecurityDescriptor = n.vaSecurityDescriptor;
				e.wszText = n.wszText;
				e.dwPID = n.dwPID;
				e.dwPoolTag = n.dwPoolTag;
				e.wszType = n.wszType;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_NETENTRY[] Map_GetNet() {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_NET>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_NETENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_NETENTRY[] m = new MAP_NETENTRY[0];
			if (!vmmi.VMMDLL_Map_GetNet(this.hVMM, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_NET nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_NET>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_NET_VERSION) {
				goto fail;
			}

			m = new MAP_NETENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_NETENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_NETENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_NETENTRY e;
				e.dwPID = n.dwPID;
				e.dwState = n.dwState;
				e.dwPoolTag = n.dwPoolTag;
				e.AF = n.AF;
				e.src.fValid = n.src_fValid;
				e.src.port = n.src_port;
				e.src.pbAddr = n.src_pbAddr;
				e.src.wszText = n.src_wszText;
				e.dst.fValid = n.dst_fValid;
				e.dst.port = n.dst_port;
				e.dst.pbAddr = n.dst_pbAddr;
				e.dst.wszText = n.dst_wszText;
				e.vaObj = n.vaObj;
				e.ftTime = n.ftTime;
				e.wszText = n.wszText;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_PHYSMEMENTRY[] Map_GetPhysMem() {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_PHYSMEM>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_PHYSMEMENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_PHYSMEMENTRY[] m = new MAP_PHYSMEMENTRY[0];
			if (!vmmi.VMMDLL_Map_GetPhysMem(this.hVMM, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_PHYSMEM nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PHYSMEM>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_PHYSMEM_VERSION) {
				goto fail;
			}

			m = new MAP_PHYSMEMENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_PHYSMEMENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PHYSMEMENTRY>((IntPtr)(pMap.ToInt64() + cbMAP +
						i * cbENTRY));
				MAP_PHYSMEMENTRY e;
				e.pa = n.pa;
				e.cb = n.cb;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_POOLENTRY[] Map_GetPool() {
			Byte[] tag = { 0, 0, 0, 0 };
			IntPtr pN = IntPtr.Zero;
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_POOL>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_POOLENTRY>();
			if (!vmmi.VMMDLL_Map_GetPool(this.hVMM, out pN, 0)) {
				return new MAP_POOLENTRY[0];
			}

			vmmi.VMMDLL_MAP_POOL nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_POOL>(pN);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_POOL_VERSION) {
				vmmi.VMMDLL_MemFree((Byte*)pN.ToPointer());
				return new MAP_POOLENTRY[0];
			}

			MAP_POOLENTRY[] eM = new MAP_POOLENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_POOLENTRY nE =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_POOLENTRY>((IntPtr)(pN.ToInt64() + cbMAP + i * cbENTRY));
				eM[i].va = nE.va;
				eM[i].cb = nE.cb;
				eM[i].tpPool = nE.tpPool;
				eM[i].tpSS = nE.tpSS;
				eM[i].dwTag = nE.dwTag;
				tag[0] = (Byte)((nE.dwTag >> 00) & 0xff);
				tag[1] = (Byte)((nE.dwTag >> 08) & 0xff);
				tag[2] = (Byte)((nE.dwTag >> 16) & 0xff);
				tag[3] = (Byte)((nE.dwTag >> 24) & 0xff);
				eM[i].sTag = Encoding.ASCII.GetString(tag);
			}

			vmmi.VMMDLL_MemFree((Byte*)pN.ToPointer());
			return eM;
		}

		public unsafe MAP_USERENTRY[] Map_GetUsers() {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_USER>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_USERENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_USERENTRY[] m = new MAP_USERENTRY[0];
			if (!vmmi.VMMDLL_Map_GetUsers(this.hVMM, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_USER nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_USER>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_USER_VERSION) {
				goto fail;
			}

			m = new MAP_USERENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_USERENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_USERENTRY>((IntPtr)(pMap.ToInt64() + cbMAP + i * cbENTRY));
				MAP_USERENTRY e;
				e.szSID = n.wszSID;
				e.wszText = n.wszText;
				e.vaRegHive = n.vaRegHive;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_SERVICEENTRY[] Map_GetServices() {
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_SERVICE>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_SERVICEENTRY>();
			IntPtr pMap = IntPtr.Zero;
			MAP_SERVICEENTRY[] m = new MAP_SERVICEENTRY[0];
			if (!vmmi.VMMDLL_Map_GetServices(this.hVMM, out pMap)) {
				goto fail;
			}

			vmmi.VMMDLL_MAP_SERVICE nM = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_SERVICE>(pMap);
			if (nM.dwVersion != vmmi.VMMDLL_MAP_SERVICE_VERSION) {
				goto fail;
			}

			m = new MAP_SERVICEENTRY[nM.cMap];
			for (Int32 i = 0; i < nM.cMap; i++) {
				vmmi.VMMDLL_MAP_SERVICEENTRY n =
					Marshal.PtrToStructure<vmmi.VMMDLL_MAP_SERVICEENTRY>((IntPtr)(pMap.ToInt64() + cbMAP +
						i * cbENTRY));
				MAP_SERVICEENTRY e;
				e.vaObj = n.vaObj;
				e.dwPID = n.dwPID;
				e.dwOrdinal = n.dwOrdinal;
				e.wszServiceName = n.wszServiceName;
				e.wszDisplayName = n.wszDisplayName;
				e.wszPath = n.wszPath;
				e.wszUserTp = n.wszUserTp;
				e.wszUserAcct = n.wszUserAcct;
				e.wszImagePath = n.wszImagePath;
				e.dwStartType = n.dwStartType;
				e.dwServiceType = n.dwServiceType;
				e.dwCurrentState = n.dwCurrentState;
				e.dwControlsAccepted = n.dwControlsAccepted;
				e.dwWin32ExitCode = n.dwWin32ExitCode;
				e.dwServiceSpecificExitCode = n.dwServiceSpecificExitCode;
				e.dwCheckPoint = n.dwCheckPoint;
				e.dwWaitHint = n.dwWaitHint;
				m[i] = e;
			}

		fail:
			vmmi.VMMDLL_MemFree((Byte*)pMap.ToPointer());
			return m;
		}

		public unsafe MAP_PFNENTRY[] Map_GetPfn(params UInt32[] pfns) {
			Boolean result;
			UInt32 cbPfns;
			Int32 cbMAP = Marshal.SizeOf<vmmi.VMMDLL_MAP_PFN>();
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_MAP_PFNENTRY>();
			if (pfns.Length == 0) {
				return new MAP_PFNENTRY[0];
			}

			Byte[] dataPfns = new Byte[pfns.Length * sizeof(UInt32)];
			Buffer.BlockCopy(pfns, 0, dataPfns, 0, dataPfns.Length);
			fixed (Byte* pbPfns = dataPfns) {
				cbPfns = (UInt32)(cbMAP + pfns.Length * cbENTRY);
				fixed (Byte* pb = new Byte[cbPfns]) {
					result =
						vmmi.VMMDLL_Map_GetPfn(this.hVMM, pbPfns, (UInt32)pfns.Length, null, ref cbPfns) &&
						vmmi.VMMDLL_Map_GetPfn(this.hVMM, pbPfns, (UInt32)pfns.Length, pb, ref cbPfns);
					if (!result) {
						return new MAP_PFNENTRY[0];
					}

					vmmi.VMMDLL_MAP_PFN pm = Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PFN>((IntPtr)pb);
					if (pm.dwVersion != vmmi.VMMDLL_MAP_PFN_VERSION) {
						return new MAP_PFNENTRY[0];
					}

					MAP_PFNENTRY[] m = new MAP_PFNENTRY[pm.cMap];
					for (Int32 i = 0; i < pm.cMap; i++) {
						vmmi.VMMDLL_MAP_PFNENTRY n =
							Marshal.PtrToStructure<vmmi.VMMDLL_MAP_PFNENTRY>((IntPtr)(pb + cbMAP + i * cbENTRY));
						MAP_PFNENTRY e = new MAP_PFNENTRY();
						e.dwPfn = n.dwPfn;
						e.tp = (MAP_PFN_TYPE)((n._u3 >> 16) & 0x07);
						e.tpExtended = (MAP_PFN_TYPEEXTENDED)n.tpExtended;
						e.vaPte = n.vaPte;
						e.OriginalPte = n.OriginalPte;
						e.fModified = ((n._u3 >> 20) & 1) == 1;
						e.fReadInProgress = ((n._u3 >> 21) & 1) == 1;
						e.fWriteInProgress = ((n._u3 >> 19) & 1) == 1;
						e.priority = (Byte)((n._u3 >> 24) & 7);
						e.fPrototype = ((n._u4 >> 57) & 1) == 1;
						if (e.tp == MAP_PFN_TYPE.Active && !e.fPrototype) {
							e.va = n.va;
							e.dwPID = n.dwPfnPte[0];
						}

						m[i] = e;
					}

					return m;
				}
			}
		}

		public unsafe REGISTRY_HIVE_INFORMATION[] RegHiveList() {
			Boolean result;
			UInt32 cHives;
			Int32 cbENTRY = Marshal.SizeOf<vmmi.VMMDLL_REGISTRY_HIVE_INFORMATION>();
			result = vmmi.VMMDLL_WinReg_HiveList(this.hVMM, null, 0, out cHives);
			if (!result || cHives == 0) {
				return new REGISTRY_HIVE_INFORMATION[0];
			}

			fixed (Byte* pb = new Byte[cHives * cbENTRY]) {
				result = vmmi.VMMDLL_WinReg_HiveList(this.hVMM, pb, cHives, out cHives);
				if (!result) {
					return new REGISTRY_HIVE_INFORMATION[0];
				}

				REGISTRY_HIVE_INFORMATION[] m = new REGISTRY_HIVE_INFORMATION[cHives];
				for (Int32 i = 0; i < cHives; i++) {
					vmmi.VMMDLL_REGISTRY_HIVE_INFORMATION n =
						Marshal.PtrToStructure<vmmi.VMMDLL_REGISTRY_HIVE_INFORMATION>((IntPtr)(pb + i * cbENTRY));
					REGISTRY_HIVE_INFORMATION e;
					if (n.wVersion != vmmi.VMMDLL_REGISTRY_HIVE_INFORMATION_VERSION) {
						return new REGISTRY_HIVE_INFORMATION[0];
					}

					e.vaCMHIVE = n.vaCMHIVE;
					e.vaHBASE_BLOCK = n.vaHBASE_BLOCK;
					e.cbLength = n.cbLength;
					e.szName = Encoding.UTF8.GetString(n.uszName);
					e.szName = e.szName.Substring(0, e.szName.IndexOf((Char)0));
					e.szNameShort = Encoding.UTF8.GetString(n.uszNameShort);
					e.szHiveRootPath = Encoding.UTF8.GetString(n.uszHiveRootPath);
					m[i] = e;
				}

				return m;
			}
		}

		public unsafe Byte[] RegHiveRead(UInt64 vaCMHIVE, UInt32 ra, UInt32 cb, UInt32 flags = 0) {
			UInt32 cbRead;
			Byte[] data = new Byte[cb];
			fixed (Byte* pb = data) {
				if (!vmmi.VMMDLL_WinReg_HiveReadEx(this.hVMM, vaCMHIVE, ra, pb, cb, out cbRead, flags)) {
					return null;
				}
			}

			if (cbRead != cb) {
				Array.Resize(ref data, (Int32)cbRead);
			}

			return data;
		}


		public unsafe Boolean RegHiveWrite(UInt64 vaCMHIVE, UInt32 ra, Byte[] data) {
			fixed (Byte* pb = data) {
				return vmmi.VMMDLL_WinReg_HiveWrite(this.hVMM, vaCMHIVE, ra, pb, (UInt32)data.Length);
			}
		}

		public unsafe REGISTRY_ENUM RegEnum(String wszFullPathKey) {
			UInt32 i, cchName, lpType, cbData = 0;
			UInt64 ftLastWriteTime;
			REGISTRY_ENUM re = new REGISTRY_ENUM();
			re.wszFullPathKey = wszFullPathKey;
			re.KeyList = new List<REGISTRY_KEY_ENUM>();
			re.ValueList = new List<REGISTRY_VALUE_ENUM>();
			fixed (Byte* pb = new Byte[0x1000]) {
				i = 0;
				cchName = 0x800;
				while (vmmi.VMMDLL_WinReg_EnumKeyExW(this.hVMM, wszFullPathKey, i, pb, ref cchName,
					out ftLastWriteTime)) {
					REGISTRY_KEY_ENUM e = new REGISTRY_KEY_ENUM();
					e.ftLastWriteTime = ftLastWriteTime;
					e.name = new String((SByte*)pb, 0, 2 * (Int32)Math.Max(1, cchName) - 2, Encoding.Unicode);
					re.KeyList.Add(e);
					i++;
					cchName = 0x800;
				}

				i = 0;
				cchName = 0x800;
				while (vmmi.VMMDLL_WinReg_EnumValueW(this.hVMM, wszFullPathKey, i, pb, ref cchName, out lpType, null,
					ref cbData)) {
					REGISTRY_VALUE_ENUM e = new REGISTRY_VALUE_ENUM();
					e.type = lpType;
					e.cbData = cbData;
					e.name = new String((SByte*)pb, 0, 2 * (Int32)Math.Max(1, cchName) - 2, Encoding.Unicode);
					re.ValueList.Add(e);
					i++;
					cchName = 0x800;
				}
			}

			return re;
		}

		public unsafe Byte[] RegValueRead(String wszFullPathKeyValue, out UInt32 tp) {
			Boolean result;
			UInt32 cb = 0;
			result = vmmi.VMMDLL_WinReg_QueryValueExW(this.hVMM, wszFullPathKeyValue, out tp, null, ref cb);
			if (!result) {
				return null;
			}

			Byte[] data = new Byte[cb];
			fixed (Byte* pb = data) {
				result = vmmi.VMMDLL_WinReg_QueryValueExW(this.hVMM, wszFullPathKeyValue, out tp, pb, ref cb);
				return result ? data : null;
			}
		}

		//---------------------------------------------------------------------
		// VFS (VIRTUAL FILE SYSTEM) FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		[StructLayout(LayoutKind.Sequential)]
		public struct VMMDLL_VFS_FILELIST_EXINFO {
			public UInt32 dwVersion;
			public Boolean fCompressed;
			public UInt64 ftCreationTime;
			public UInt64 ftLastAccessTime;
			public UInt64 ftLastWriteTime;
		}


		//---------------------------------------------------------------------
		// MEMORY SEARCH FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public struct VMMDLL_MEM_SEARCHENTRY {
			public UInt32 cbAlign;
			public Byte[] pbSearch;
			public Byte[] pbSearchSkipMask;
		}


		//---------------------------------------------------------------------
		// PROCESS FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public struct PROCESS_INFORMATION {
			public Boolean fValid;
			public UInt32 tpMemoryModel;
			public UInt32 tpSystem;
			public Boolean fUserOnly;
			public UInt32 dwPID;
			public UInt32 dwPPID;
			public UInt32 dwState;
			public String szName;
			public String szNameLong;
			public UInt64 paDTB;
			public UInt64 paDTB_UserOpt;
			public UInt64 vaEPROCESS;
			public UInt64 vaPEB;
			public Boolean fWow64;
			public UInt32 vaPEB32;
			public UInt32 dwSessionId;
			public UInt64 qwLUID;
			public String szSID;
			public UInt32 IntegrityLevel;
		}

		public struct IMAGE_SECTION_HEADER {
			public String Name;
			public UInt32 MiscPhysicalAddressOrVirtualSize;
			public UInt32 VirtualAddress;
			public UInt32 SizeOfRawData;
			public UInt32 PointerToRawData;
			public UInt32 PointerToRelocations;
			public UInt32 PointerToLinenumbers;
			public UInt16 NumberOfRelocations;
			public UInt16 NumberOfLinenumbers;
			public UInt32 Characteristics;
		}

		public struct IMAGE_DATA_DIRECTORY {
			public String name;
			public UInt32 VirtualAddress;
			public UInt32 Size;
		}

		public struct MAP_PTEENTRY {
			public UInt64 vaBase;
			public UInt64 vaEnd;
			public UInt64 cbSize;
			public UInt64 cPages;
			public UInt64 fPage;
			public Boolean fWoW64;
			public String wszText;
			public UInt32 cSoftware;
		}

		public struct MAP_VADENTRY {
			public UInt64 vaStart;
			public UInt64 vaEnd;
			public UInt64 vaVad;
			public UInt64 cbSize;
			public UInt32 VadType;
			public UInt32 Protection;
			public Boolean fImage;
			public Boolean fFile;
			public Boolean fPageFile;
			public Boolean fPrivateMemory;
			public Boolean fTeb;
			public Boolean fStack;
			public UInt32 fSpare;
			public UInt32 HeapNum;
			public Boolean fHeap;
			public UInt32 cwszDescription;
			public UInt32 CommitCharge;
			public Boolean MemCommit;
			public UInt32 u2;
			public UInt32 cbPrototypePte;
			public UInt64 vaPrototypePte;
			public UInt64 vaSubsection;
			public String wszText;
			public UInt64 vaFileObject;
			public UInt32 cVadExPages;
			public UInt32 cVadExPagesBase;
		}

		public struct MAP_VADEXENTRY_PROTOTYPE {
			public UInt32 tp;
			public UInt64 pa;
			public UInt64 pte;
		}

		public struct MAP_VADEXENTRY {
			public UInt32 tp;
			public UInt32 iPML;
			public UInt64 va;
			public UInt64 pa;
			public UInt64 pte;
			public UInt32 pteFlags;
			public MAP_VADEXENTRY_PROTOTYPE proto;
			public UInt64 vaVadBase;
		}

		public struct MODULEENTRY_DEBUGINFO {
			public Boolean fValid;
			public UInt32 dwAge;
			public String wszGuid;
			public String wszPdbFilename;
		}

		public struct MODULEENTRY_VERSIONINFO {
			public Boolean fValid;
			public String wszCompanyName;
			public String wszFileDescription;
			public String wszFileVersion;
			public String wszInternalName;
			public String wszLegalCopyright;
			public String wszFileOriginalFilename;
			public String wszProductName;
			public String wszProductVersion;
		}

		public struct MAP_MODULEENTRY {
			public Boolean fValid;
			public UInt64 vaBase;
			public UInt64 vaEntry;
			public UInt32 cbImageSize;
			public Boolean fWow64;
			public String wszText;
			public String wszFullName;
			public UInt32 tp;
			public UInt32 cbFileSizeRaw;
			public UInt32 cSection;
			public UInt32 cEAT;
			public UInt32 cIAT;
			public MODULEENTRY_DEBUGINFO DebugInfo;
			public MODULEENTRY_VERSIONINFO VersionInfo;
		}

		public struct MAP_UNLOADEDMODULEENTRY {
			public UInt64 vaBase;
			public UInt32 cbImageSize;
			public Boolean fWow64;
			public String wszText;
			public UInt32 dwCheckSum; // user-mode only
			public UInt32 dwTimeDateStamp; // user-mode only
			public UInt64 ftUnload; // kernel-mode only
		}

		public struct MAP_EATINFO {
			public Boolean fValid;
			public UInt64 vaModuleBase;
			public UInt64 vaAddressOfFunctions;
			public UInt64 vaAddressOfNames;
			public UInt32 cNumberOfFunctions;
			public UInt32 cNumberOfForwardedFunctions;
			public UInt32 cNumberOfNames;
			public UInt32 dwOrdinalBase;
		}

		public struct MAP_EATENTRY {
			public UInt64 vaFunction;
			public UInt32 dwOrdinal;
			public UInt32 oFunctionsArray;
			public UInt32 oNamesArray;
			public String wszFunction;
			public String wszForwardedFunction;
		}

		public struct MAP_IATENTRY {
			public UInt64 vaFunction;
			public UInt64 vaModule;
			public String wszFunction;
			public String wszModule;
			public Boolean f32;
			public UInt16 wHint;
			public UInt32 rvaFirstThunk;
			public UInt32 rvaOriginalFirstThunk;
			public UInt32 rvaNameModule;
			public UInt32 rvaNameFunction;
		}

		public struct MAP_HEAPENTRY {
			public UInt64 va;
			public UInt32 tpHeap;
			public Boolean f32;
			public UInt32 iHeapNum;
		}

		public struct MAP_HEAPSEGMENTENTRY {
			public UInt64 va;
			public UInt32 cb;
			public UInt32 tpHeapSegment;
			public UInt32 iHeapNum;
		}

		public struct MAP_HEAP {
			public MAP_HEAPENTRY[] heaps;
			public MAP_HEAPSEGMENTENTRY[] segments;
		}

		public struct MAP_HEAPALLOCENTRY {
			public UInt64 va;
			public UInt32 cb;
			public UInt32 tp;
		}

		public struct MAP_THREADENTRY {
			public UInt32 dwTID;
			public UInt32 dwPID;
			public UInt32 dwExitStatus;
			public Byte bState;
			public Byte bRunning;
			public Byte bPriority;
			public Byte bBasePriority;
			public UInt64 vaETHREAD;
			public UInt64 vaTeb;
			public UInt64 ftCreateTime;
			public UInt64 ftExitTime;
			public UInt64 vaStartAddress;
			public UInt64 vaWin32StartAddress;
			public UInt64 vaStackBaseUser;
			public UInt64 vaStackLimitUser;
			public UInt64 vaStackBaseKernel;
			public UInt64 vaStackLimitKernel;
			public UInt64 vaTrapFrame;
			public UInt64 vaImpersonationToken;
			public UInt64 vaRIP;
			public UInt64 vaRSP;
			public UInt64 qwAffinity;
			public UInt32 dwUserTime;
			public UInt32 dwKernelTime;
			public Byte bSuspendCount;
			public Byte bWaitReason;
		}

		public struct MAP_HANDLEENTRY {
			public UInt64 vaObject;
			public UInt32 dwHandle;
			public UInt32 dwGrantedAccess;
			public UInt32 iType;
			public UInt64 qwHandleCount;
			public UInt64 qwPointerCount;
			public UInt64 vaObjectCreateInfo;
			public UInt64 vaSecurityDescriptor;
			public String wszText;
			public UInt32 dwPID;
			public UInt32 dwPoolTag;
			public String wszType;
		}

		public struct MAP_NETENTRY_ADDR {
			public Boolean fValid;
			public UInt16 port;
			public Byte[] pbAddr;
			public String wszText;
		}

		public struct MAP_NETENTRY {
			public UInt32 dwPID;
			public UInt32 dwState;
			public UInt32 dwPoolTag;
			public UInt16 AF;
			public MAP_NETENTRY_ADDR src;
			public MAP_NETENTRY_ADDR dst;
			public UInt64 vaObj;
			public UInt64 ftTime;
			public String wszText;
		}

		public struct MAP_PHYSMEMENTRY {
			public UInt64 pa;
			public UInt64 cb;
		}

		public struct MAP_POOLENTRY {
			public UInt64 va;
			public UInt32 cb;
			public UInt32 fAlloc;
			public UInt32 tpPool;
			public UInt32 tpSS;
			public UInt32 dwTag;
			public String sTag;
		}

		public struct MAP_USERENTRY {
			public String szSID;
			public String wszText;
			public UInt64 vaRegHive;
		}

		public struct MAP_SERVICEENTRY {
			public UInt64 vaObj;
			public UInt32 dwPID;
			public UInt32 dwOrdinal;
			public String wszServiceName;
			public String wszDisplayName;
			public String wszPath;
			public String wszUserTp;
			public String wszUserAcct;
			public String wszImagePath;
			public UInt32 dwStartType;
			public UInt32 dwServiceType;
			public UInt32 dwCurrentState;
			public UInt32 dwControlsAccepted;
			public UInt32 dwWin32ExitCode;
			public UInt32 dwServiceSpecificExitCode;
			public UInt32 dwCheckPoint;
			public UInt32 dwWaitHint;
		}

		public struct MAP_PFNENTRY {
			public UInt32 dwPfn;
			public MAP_PFN_TYPE tp;
			public MAP_PFN_TYPEEXTENDED tpExtended;
			public UInt64 va;
			public UInt64 vaPte;
			public UInt64 OriginalPte;
			public UInt32 dwPID;
			public Boolean fPrototype;
			public Boolean fModified;
			public Boolean fReadInProgress;
			public Boolean fWriteInProgress;
			public Byte priority;
		}


		//---------------------------------------------------------------------
		// REGISTRY FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------

		public struct REGISTRY_HIVE_INFORMATION {
			public UInt64 vaCMHIVE;
			public UInt64 vaHBASE_BLOCK;
			public UInt32 cbLength;
			public String szName;
			public String szNameShort;
			public String szHiveRootPath;
		}

		public struct REGISTRY_KEY_ENUM {
			public String name;
			public UInt64 ftLastWriteTime;
		}

		public struct REGISTRY_VALUE_ENUM {
			public String name;
			public UInt32 type;
			public UInt32 cbData;
		}

		public struct REGISTRY_ENUM {
			public String wszFullPathKey;
			public List<REGISTRY_KEY_ENUM> KeyList;
			public List<REGISTRY_VALUE_ENUM> ValueList;
		}
	}

	public sealed class VmmScatter : IDisposable {
		//---------------------------------------------------------------------
		// MEMORY NEW SCATTER READ/WRITE FUNCTIONALITY BELOW:
		//---------------------------------------------------------------------
		private Boolean disposed;
		private IntPtr hS = IntPtr.Zero;

		private VmmScatter() {
			;
		}

		internal VmmScatter(IntPtr hS) {
			this.hS = hS;
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~VmmScatter() {
			this.Dispose(false);
		}

		private void Dispose(Boolean disposing) {
			if (!this.disposed) {
				vmmi.VMMDLL_Scatter_CloseHandle(this.hS);
				this.hS = IntPtr.Zero;
				this.disposed = true;
			}
		}

		public void Close() {
			this.Dispose(true);
		}

		public unsafe Byte[] Read(UInt64 qwA, UInt32 cb) {
			UInt32 cbRead;
			Byte[] data = new Byte[cb];
			fixed (Byte* pb = data) {
				if (!vmmi.VMMDLL_Scatter_Read(this.hS, qwA, cb, pb, out cbRead)) {
					return null;
				}
			}

			if (cbRead != cb) {
				Array.Resize(ref data, (Int32)cbRead);
			}

			return data;
		}

		public unsafe Boolean ReadStruct<T>(UInt64 qwA, out T result)
			where T : unmanaged {
			UInt32 cb = (UInt32)sizeof(T);
			UInt32 cbRead;
			result = default;
			fixed (T* pb = &result) {
				if (!vmmi.VMMDLL_Scatter_Read(this.hS, qwA, cb, (Byte*)pb, out cbRead)) {
					return false;
				}
			}

			if (cbRead != cb) {
				return false;
			}

			return true;
		}

		public Boolean Prepare(UInt64 qwA, UInt32 cb) {
			return vmmi.VMMDLL_Scatter_Prepare(this.hS, qwA, cb);
		}

		public unsafe Boolean PrepareWrite(UInt64 qwA, Byte[] data) {
			fixed (Byte* pb = data) {
				return vmmi.VMMDLL_Scatter_PrepareWrite(this.hS, qwA, pb, (UInt32)data.Length);
			}
		}

		public unsafe Boolean PrepareWriteStruct<T>(UInt64 qwA, T value)
			where T : unmanaged {
			UInt32 cb = (UInt32)sizeof(T);
			Byte* pb = (Byte*)&value;
			return vmmi.VMMDLL_Scatter_PrepareWrite(this.hS, qwA, pb, cb);
		}

		public Boolean Execute() {
			return vmmi.VMMDLL_Scatter_Execute(this.hS);
		}

		public Boolean Clear(UInt32 dwPID, UInt32 flags) {
			return vmmi.VMMDLL_Scatter_Clear(this.hS, dwPID, flags);
		}
	}

	internal static class lci {
		[DllImport("leechcore.dll", EntryPoint = "LcCreate")]
		public static extern IntPtr LcCreate(ref LeechCore.LC_CONFIG pLcCreateConfig);

		[DllImport("leechcore.dll", EntryPoint = "LcCreateEx")]
		public static extern IntPtr LcCreateEx(ref LeechCore.LC_CONFIG pLcCreateConfig, out IntPtr ppLcCreateErrorInfo);

		[DllImport("leechcore.dll", EntryPoint = "LcClose")]
		internal static extern void LcClose(IntPtr hLC);

		[DllImport("leechcore.dll", EntryPoint = "LcMemFree")]
		internal static extern void LcMemFree(IntPtr pv);

		[DllImport("leechcore.dll", EntryPoint = "LcAllocScatter1")]
		internal static extern Boolean LcAllocScatter1(UInt32 cMEMs, out IntPtr pppMEMs);

		[DllImport("leechcore.dll", EntryPoint = "LcRead")]
		internal static extern unsafe Boolean LcRead(IntPtr hLC, UInt64 pa, UInt32 cb, Byte* pb);

		[DllImport("leechcore.dll", EntryPoint = "LcReadScatter")]
		internal static extern void LcReadScatter(IntPtr hLC, UInt32 cMEMs, IntPtr ppMEMs);

		[DllImport("leechcore.dll", EntryPoint = "LcWrite")]
		internal static extern unsafe Boolean LcWrite(IntPtr hLC, UInt64 pa, UInt32 cb, Byte* pb);

		[DllImport("leechcore.dll", EntryPoint = "LcWriteScatter")]
		internal static extern void LcWriteScatter(IntPtr hLC, UInt32 cMEMs, IntPtr ppMEMs);

		[DllImport("leechcore.dll", EntryPoint = "LcGetOption")]
		public static extern Boolean GetOption(IntPtr hLC, UInt64 fOption, out UInt64 pqwValue);

		[DllImport("leechcore.dll", EntryPoint = "LcSetOption")]
		public static extern Boolean SetOption(IntPtr hLC, UInt64 fOption, UInt64 qwValue);

		[DllImport("leechcore.dll", EntryPoint = "LcCommand")]
		internal static extern unsafe Boolean LcCommand(IntPtr hLC, UInt64 fOption, UInt32 cbDataIn, Byte* pbDataIn,
			out IntPtr ppbDataOut, out UInt32 pcbDataOut);

		[StructLayout(LayoutKind.Sequential)]
		internal struct LC_CONFIG_ERRORINFO {
			internal UInt32 dwVersion;
			internal UInt32 cbStruct;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			internal UInt32[] _FutureUse;

			internal Boolean fUserInputRequest;

			internal UInt32 cwszUserText;
			// szUserText
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct LC_MEM_SCATTER {
			internal UInt32 version;
			internal Boolean f;
			internal UInt64 qwA;
			internal IntPtr pb;
			internal UInt32 cb;
			internal UInt32 iStack;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
			internal UInt64[] vStack;
		}
	}

	internal static class vmmi {
		internal const UInt64 MAX_PATH = 260;
		internal const UInt32 VMMDLL_MAP_PTE_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_VAD_VERSION = 6;
		internal const UInt32 VMMDLL_MAP_VADEX_VERSION = 4;
		internal const UInt32 VMMDLL_MAP_MODULE_VERSION = 6;
		internal const UInt32 VMMDLL_MAP_UNLOADEDMODULE_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_EAT_VERSION = 3;
		internal const UInt32 VMMDLL_MAP_IAT_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_HEAP_VERSION = 4;
		internal const UInt32 VMMDLL_MAP_HEAPALLOC_VERSION = 1;
		internal const UInt32 VMMDLL_MAP_THREAD_VERSION = 4;
		internal const UInt32 VMMDLL_MAP_HANDLE_VERSION = 3;
		internal const UInt32 VMMDLL_MAP_NET_VERSION = 3;
		internal const UInt32 VMMDLL_MAP_PHYSMEM_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_POOL_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_USER_VERSION = 2;
		internal const UInt32 VMMDLL_MAP_PFN_VERSION = 1;
		internal const UInt32 VMMDLL_MAP_SERVICE_VERSION = 3;
		internal const UInt32 VMMDLL_MEM_SEARCH_VERSION = 0xfe3e0002;
		internal const UInt32 VMMDLL_REGISTRY_HIVE_INFORMATION_VERSION = 4;


		// VFS (VIRTUAL FILE SYSTEM) FUNCTIONALITY BELOW:

		internal const UInt32 VMMDLL_VFS_FILELIST_EXINFO_VERSION = 1;
		internal const UInt32 VMMDLL_VFS_FILELIST_VERSION = 2;

		internal const UInt64 VMMDLL_PROCESS_INFORMATION_MAGIC = 0xc0ffee663df9301e;
		internal const UInt16 VMMDLL_PROCESS_INFORMATION_VERSION = 7;


		[DllImport("vmm.dll", EntryPoint = "VMMDLL_InitializeEx")]
		internal static extern IntPtr VMMDLL_InitializeEx(
			Int32 argc,
			String[] argv,
			out IntPtr ppLcErrorInfo);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_CloseAll")]
		public static extern void VMMDLL_CloseAll();

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Close")]
		public static extern void VMMDLL_Close(
			IntPtr hVMM);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ConfigGet")]
		public static extern Boolean VMMDLL_ConfigGet(
			IntPtr hVMM,
			UInt64 fOption,
			out UInt64 pqwValue);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ConfigSet")]
		public static extern Boolean VMMDLL_ConfigSet(
			IntPtr hVMM,
			UInt64 fOption,
			UInt64 qwValue);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemFree")]
		internal static extern unsafe void VMMDLL_MemFree(
			Byte* pvMem);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_VfsListU")]
		internal static extern Boolean VMMDLL_VfsList(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPUTF8Str)] String wcsPath,
			ref VMMDLL_VFS_FILELIST pFileList);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_VfsReadU")]
		internal static extern unsafe UInt32 VMMDLL_VfsRead(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPUTF8Str)] String wcsFileName,
			Byte* pb,
			UInt32 cb,
			out UInt32 pcbRead,
			UInt64 cbOffset);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_VfsWriteU")]
		internal static extern unsafe UInt32 VMMDLL_VfsWrite(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPUTF8Str)] String wcsFileName,
			Byte* pb,
			UInt32 cb,
			out UInt32 pcbRead,
			UInt64 cbOffset);


		// PLUGIN FUNCTIONALITY BELOW:

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_InitializePlugins")]
		public static extern Boolean VMMDLL_InitializePlugins(IntPtr hVMM);


		// MEMORY READ/WRITE FUNCTIONALITY BELOW:

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemReadScatter")]
		internal static extern UInt32 VMMDLL_MemReadScatter(
			IntPtr hVMM,
			UInt32 dwPID,
			IntPtr ppMEMs,
			UInt32 cpMEMs,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemReadEx")]
		internal static extern unsafe Boolean VMMDLL_MemReadEx(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt64 qwA,
			Byte* pb,
			UInt32 cb,
			out UInt32 pcbReadOpt,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemPrefetchPages")]
		internal static extern unsafe Boolean VMMDLL_MemPrefetchPages(
			IntPtr hVMM,
			UInt32 dwPID,
			Byte* pPrefetchAddresses,
			UInt32 cPrefetchAddresses);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemWrite")]
		internal static extern unsafe Boolean VMMDLL_MemWrite(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt64 qwA,
			Byte* pb,
			UInt32 cb);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemVirt2Phys")]
		public static extern Boolean VMMDLL_MemVirt2Phys(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt64 qwVA,
			out UInt64 pqwPA
		);


		// MEMORY NEW SCATTER READ/WRITE FUNCTIONALITY BELOW:

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Initialize")]
		internal static extern IntPtr VMMDLL_Scatter_Initialize(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Prepare")]
		internal static extern Boolean VMMDLL_Scatter_Prepare(
			IntPtr hS,
			UInt64 va,
			UInt32 cb);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_PrepareWrite")]
		internal static extern unsafe Boolean VMMDLL_Scatter_PrepareWrite(
			IntPtr hS,
			UInt64 va,
			Byte* pb,
			UInt32 cb);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_ExecuteRead")]
		internal static extern Boolean VMMDLL_Scatter_ExecuteRead(
			IntPtr hS);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Execute")]
		internal static extern Boolean VMMDLL_Scatter_Execute(
			IntPtr hS);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Read")]
		internal static extern unsafe Boolean VMMDLL_Scatter_Read(
			IntPtr hS,
			UInt64 va,
			UInt32 cb,
			Byte* pb,
			out UInt32 pcbRead);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Clear")]
		public static extern Boolean SVMMDLL_Scatter_Clear(IntPtr hS, UInt32 dwPID, UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_Clear")]
		internal static extern Boolean VMMDLL_Scatter_Clear(
			IntPtr hS,
			UInt32 dwPID,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Scatter_CloseHandle")]
		internal static extern void VMMDLL_Scatter_CloseHandle(
			IntPtr hS);


		// PROCESS FUNCTIONALITY BELOW:

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PidList")]
		internal static extern unsafe Boolean VMMDLL_PidList(IntPtr hVMM, Byte* pPIDs, ref UInt64 pcPIDs);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PidGetFromName")]
		public static extern Boolean VMMDLL_PidGetFromName(IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPStr)] String szProcName,
			out UInt32 pdwPID);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetProcAddressW")]
		public static extern UInt64 VMMDLL_ProcessGetProcAddress(IntPtr hVMM, UInt32 pid,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModuleName,
			[MarshalAs(UnmanagedType.LPStr)] String szFunctionName);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetModuleBaseW")]
		public static extern UInt64 VMMDLL_ProcessGetModuleBase(IntPtr hVMM, UInt32 pid,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModuleName);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetInformation")]
		internal static extern unsafe Boolean VMMDLL_ProcessGetInformation(
			IntPtr hVMM,
			UInt32 dwPID,
			Byte* pProcessInformation,
			ref UInt64 pcbProcessInformation);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetInformationString")]
		internal static extern unsafe Byte* VMMDLL_ProcessGetInformationString(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt32 fOptionString);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetDirectoriesW")]
		internal static extern unsafe Boolean VMMDLL_ProcessGetDirectories(
			IntPtr hVMM,
			UInt32 dwPID,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModule,
			Byte* pData);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_ProcessGetSectionsW")]
		internal static extern unsafe Boolean VMMDLL_ProcessGetSections(
			IntPtr hVMM,
			UInt32 dwPID,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModule,
			Byte* pData,
			UInt32 cData,
			out UInt32 pcData);


		// WINDOWS SPECIFIC DEBUGGING / SYMBOL FUNCTIONALITY BELOW:

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PdbLoad")]
		internal static extern unsafe Boolean VMMDLL_PdbLoad(
			IntPtr hVMM,
			UInt32 dwPID,
			UInt64 vaModuleBase,
			Byte* pModuleMapEntry);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PdbSymbolName")]
		internal static extern unsafe Boolean VMMDLL_PdbSymbolName(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPStr)] String szModule,
			UInt64 cbSymbolAddressOrOffset,
			Byte* szSymbolName,
			out UInt32 pdwSymbolDisplacement);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PdbSymbolAddress")]
		public static extern Boolean VMMDLL_PdbSymbolAddress(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPStr)] String szModule,
			[MarshalAs(UnmanagedType.LPStr)] String szSymbolName,
			out UInt64 pvaSymbolAddress);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PdbTypeSize")]
		public static extern Boolean VMMDLL_PdbTypeSize(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPStr)] String szModule,
			[MarshalAs(UnmanagedType.LPStr)] String szTypeName,
			out UInt32 pcbTypeSize);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_PdbTypeChildOffset")]
		public static extern Boolean VMMDLL_PdbTypeChildOffset(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPStr)] String szModule,
			[MarshalAs(UnmanagedType.LPStr)] String szTypeName,
			[MarshalAs(UnmanagedType.LPStr)] String wszTypeChildName,
			out UInt32 pcbTypeChildOffset);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetPteW")]
		internal static extern Boolean VMMDLL_Map_GetPte(
			IntPtr hVMM,
			UInt32 dwPid,
			Boolean fIdentifyModules,
			out IntPtr ppPteMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetVadW")]
		internal static extern Boolean VMMDLL_Map_GetVad(
			IntPtr hVMM,
			UInt32 dwPid,
			Boolean fIdentifyModules,
			out IntPtr ppVadMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetVadEx")]
		internal static extern Boolean VMMDLL_Map_GetVadEx(
			IntPtr hVMM,
			UInt32 dwPid,
			UInt32 oPage,
			UInt32 cPage,
			out IntPtr ppVadExMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetModuleW")]
		internal static extern Boolean VMMDLL_Map_GetModule(
			IntPtr hVMM,
			UInt32 dwPid,
			out IntPtr ppModuleMap,
			UInt32 flags);

		// VMMDLL_Map_GetModuleFromName

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetModuleFromNameW")]
		internal static extern Boolean VMMDLL_Map_GetModuleFromName(
			IntPtr hVMM,
			UInt32 dwPID,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModuleName,
			out IntPtr ppModuleMapEntry,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetUnloadedModuleW")]
		internal static extern Boolean VMMDLL_Map_GetUnloadedModule(
			IntPtr hVMM,
			UInt32 dwPid,
			out IntPtr ppModuleMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetEATW")]
		internal static extern Boolean VMMDLL_Map_GetEAT(
			IntPtr hVMM,
			UInt32 dwPid,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModuleName,
			out IntPtr ppEatMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetIATW")]
		internal static extern Boolean VMMDLL_Map_GetIAT(
			IntPtr hVMM,
			UInt32 dwPid,
			[MarshalAs(UnmanagedType.LPWStr)] String wszModuleName,
			out IntPtr ppIatMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetHeap")]
		internal static extern Boolean VMMDLL_Map_GetHeap(
			IntPtr hVMM,
			UInt32 dwPid,
			out IntPtr ppHeapMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetHeapAlloc")]
		internal static extern Boolean VMMDLL_Map_GetHeapAlloc(
			IntPtr hVMM,
			UInt32 dwPid,
			UInt64 qwHeapNumOrAddress,
			out IntPtr ppHeapAllocMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetThread")]
		internal static extern Boolean VMMDLL_Map_GetThread(
			IntPtr hVMM,
			UInt32 dwPid,
			out IntPtr ppThreadMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetHandleW")]
		internal static extern Boolean VMMDLL_Map_GetHandle(
			IntPtr hVMM,
			UInt32 dwPid,
			out IntPtr ppHandleMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetNetW")]
		internal static extern Boolean VMMDLL_Map_GetNet(
			IntPtr hVMM,
			out IntPtr ppNetMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetPhysMem")]
		internal static extern Boolean VMMDLL_Map_GetPhysMem(
			IntPtr hVMM,
			out IntPtr ppPhysMemMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetPool")]
		internal static extern Boolean VMMDLL_Map_GetPool(
			IntPtr hVMM,
			out IntPtr ppHeapAllocMap,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetUsersW")]
		internal static extern Boolean VMMDLL_Map_GetUsers(
			IntPtr hVMM,
			out IntPtr ppUserMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetServicesW")]
		internal static extern Boolean VMMDLL_Map_GetServices(
			IntPtr hVMM,
			out IntPtr ppServiceMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_Map_GetPfn")]
		internal static extern unsafe Boolean VMMDLL_Map_GetPfn(
			IntPtr hVMM,
			Byte* pPfns,
			UInt32 cPfns,
			Byte* pPfnMap,
			ref UInt32 pcbPfnMap);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_HiveList")]
		internal static extern unsafe Boolean VMMDLL_WinReg_HiveList(
			IntPtr hVMM,
			Byte* pHives,
			UInt32 cHives,
			out UInt32 pcHives);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_HiveReadEx")]
		internal static extern unsafe Boolean VMMDLL_WinReg_HiveReadEx(
			IntPtr hVMM,
			UInt64 vaCMHive,
			UInt32 ra,
			Byte* pb,
			UInt32 cb,
			out UInt32 pcbReadOpt,
			UInt32 flags);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_HiveWrite")]
		internal static extern unsafe Boolean VMMDLL_WinReg_HiveWrite(
			IntPtr hVMM,
			UInt64 vaCMHive,
			UInt32 ra,
			Byte* pb,
			UInt32 cb);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_EnumKeyExW")]
		internal static extern unsafe Boolean VMMDLL_WinReg_EnumKeyExW(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPWStr)] String wszFullPathKey,
			UInt32 dwIndex,
			Byte* lpName,
			ref UInt32 lpcchName,
			out UInt64 lpftLastWriteTime);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_EnumValueW")]
		internal static extern unsafe Boolean VMMDLL_WinReg_EnumValueW(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPWStr)] String wszFullPathKey,
			UInt32 dwIndex,
			Byte* lpValueName,
			ref UInt32 lpcchValueName,
			out UInt32 lpType,
			Byte* lpData,
			ref UInt32 lpcbData);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_WinReg_QueryValueExW")]
		internal static extern unsafe Boolean VMMDLL_WinReg_QueryValueExW(
			IntPtr hVMM,
			[MarshalAs(UnmanagedType.LPWStr)] String wszFullPathKeyValue,
			out UInt32 lpType,
			Byte* lpData,
			ref UInt32 lpcbData);

		[DllImport("vmm.dll", EntryPoint = "VMMDLL_MemSearch")]
		internal static extern Boolean VMMDLL_MemSearch(
			IntPtr hVMM,
			UInt32 dwPID,
			ref VMMDLL_MEM_SEARCH_CONTEXT ctx,
			out IntPtr ppva,
			out UInt32 pcva);

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_VFS_FILELIST {
			internal UInt32 dwVersion;
			internal UInt32 _Reserved;
			internal IntPtr pfnAddFile;
			internal IntPtr pfnAddDirectory;
			internal UInt64 h;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct VMMDLL_PROCESS_INFORMATION {
			internal UInt64 magic;
			internal UInt16 wVersion;
			internal UInt16 wSize;
			internal UInt32 tpMemoryModel;
			internal UInt32 tpSystem;
			internal Boolean fUserOnly;
			internal UInt32 dwPID;
			internal UInt32 dwPPID;
			internal UInt32 dwState;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			internal String szName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			internal String szNameLong;

			internal UInt64 paDTB;
			internal UInt64 paDTB_UserOpt;
			internal UInt64 vaEPROCESS;
			internal UInt64 vaPEB;
			internal UInt64 _Reserved1;
			internal Boolean fWow64;
			internal UInt32 vaPEB32;
			internal UInt32 dwSessionId;
			internal UInt64 qwLUID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal String szSID;

			internal UInt32 IntegrityLevel;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct VMMDLL_IMAGE_SECTION_HEADER {
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			internal String Name;

			internal UInt32 MiscPhysicalAddressOrVirtualSize;
			internal UInt32 VirtualAddress;
			internal UInt32 SizeOfRawData;
			internal UInt32 PointerToRawData;
			internal UInt32 PointerToRelocations;
			internal UInt32 PointerToLinenumbers;
			internal UInt16 NumberOfRelocations;
			internal UInt16 NumberOfLinenumbers;
			internal UInt32 Characteristics;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_IMAGE_DATA_DIRECTORY {
			internal UInt32 VirtualAddress;
			internal UInt32 Size;
		}


		// VMMDLL_Map_GetPte

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PTEENTRY {
			internal UInt64 vaBase;
			internal UInt64 cPages;
			internal UInt64 fPage;
			internal Boolean fWoW64;
			internal UInt32 _FutureUse1;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt32 _Reserved1;
			internal UInt32 cSoftware;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PTE {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetVad

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_VADENTRY {
			internal UInt64 vaStart;
			internal UInt64 vaEnd;
			internal UInt64 vaVad;
			internal UInt32 dw0;
			internal UInt32 dw1;
			internal UInt32 u2;
			internal UInt32 cbPrototypePte;
			internal UInt64 vaPrototypePte;
			internal UInt64 vaSubsection;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt32 _FutureUse1;
			internal UInt32 _Reserved1;
			internal UInt64 vaFileObject;
			internal UInt32 cVadExPages;
			internal UInt32 cVadExPagesBase;
			internal UInt64 _Reserved2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_VAD {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			internal UInt32[] _Reserved1;

			internal UInt32 cPage;
			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetVadEx

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_VADEXENTRY {
			internal UInt32 tp;
			internal Byte iPML;
			internal Byte pteFlags;
			internal UInt16 _Reserved2;
			internal UInt64 va;
			internal UInt64 pa;
			internal UInt64 pte;
			internal UInt32 _Reserved1;
			internal UInt32 proto_tp;
			internal UInt64 proto_pa;
			internal UInt64 proto_pte;
			internal UInt64 vaVadBase;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_VADEX {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			internal UInt32[] _Reserved1;

			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetModule
		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_MODULEENTRY_DEBUGINFO {
			internal UInt32 dwAge;
			internal UInt32 _Reserved;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			internal Byte[] Guid;

			[MarshalAs(UnmanagedType.LPWStr)] internal String wszGuid;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszPdbFilename;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_MODULEENTRY_VERSIONINFO {
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszCompanyName;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFileDescription;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFileVersion;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszInternalName;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszLegalCopyright;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFileOriginalFilename;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszProductName;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszProductVersion;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_MODULEENTRY {
			internal UInt64 vaBase;
			internal UInt64 vaEntry;
			internal UInt32 cbImageSize;
			internal Boolean fWow64;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt32 _Reserved3;
			internal UInt32 _Reserved4;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFullName;
			internal UInt32 tp;
			internal UInt32 cbFileSizeRaw;
			internal UInt32 cSection;
			internal UInt32 cEAT;
			internal UInt32 cIAT;
			internal UInt32 _Reserved2;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal UInt64[] _Reserved1;

			internal IntPtr pExDebugInfo;
			internal IntPtr pExVersionInfo;
		}

		internal struct VMMDLL_MAP_MODULE {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetUnloadedModule

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_UNLOADEDMODULEENTRY {
			internal UInt64 vaBase;
			internal UInt32 cbImageSize;
			internal Boolean fWow64;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt32 _FutureUse1;
			internal UInt32 dwCheckSum;
			internal UInt32 dwTimeDateStamp;
			internal UInt32 _Reserved1;
			internal UInt64 ftUnload;
		}

		internal struct VMMDLL_MAP_UNLOADEDMODULE {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetEAT

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_EATENTRY {
			internal UInt64 vaFunction;
			internal UInt32 dwOrdinal;
			internal UInt32 oFunctionsArray;
			internal UInt32 oNamesArray;
			internal UInt32 _FutureUse1;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFunction;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszForwardedFunction;
		}

		internal struct VMMDLL_MAP_EAT {
			internal UInt32 dwVersion;
			internal UInt32 dwOrdinalBase;
			internal UInt32 cNumberOfNames;
			internal UInt32 cNumberOfFunctions;
			internal UInt32 cNumberOfForwardedFunctions;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal UInt32[] _Reserved1;

			internal UInt64 vaModuleBase;
			internal UInt64 vaAddressOfFunctions;
			internal UInt64 vaAddressOfNames;
			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetIAT

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_IATENTRY {
			internal UInt64 vaFunction;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszFunction;
			internal UInt32 _FutureUse1;
			internal UInt32 _FutureUse2;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszModule;
			internal Boolean f32;
			internal UInt16 wHint;
			internal UInt16 _Reserved1;
			internal UInt32 rvaFirstThunk;
			internal UInt32 rvaOriginalFirstThunk;
			internal UInt32 rvaNameModule;
			internal UInt32 rvaNameFunction;
		}

		internal struct VMMDLL_MAP_IAT {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 vaModuleBase;
			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetHeap

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HEAPENTRY {
			internal UInt64 va;
			internal UInt32 tp;
			internal Boolean f32;
			internal UInt32 iHeap;
			internal UInt32 dwHeapNum;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HEAPSEGMENTENTRY {
			internal UInt64 va;
			internal UInt32 cb;
			internal UInt16 tp;
			internal UInt16 iHeap;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HEAP {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
			internal UInt32[] _Reserved1;

			internal IntPtr pSegments;
			internal UInt32 cSegments;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetHeapAlloc

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HEAPALLOCENTRY {
			internal UInt64 va;
			internal UInt32 cb;
			internal UInt32 tp;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HEAPALLOC {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
			internal UInt32[] _Reserved1;

			internal IntPtr _Reserved20;
			internal IntPtr _Reserved21;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetThread

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_THREADENTRY {
			internal UInt32 dwTID;
			internal UInt32 dwPID;
			internal UInt32 dwExitStatus;
			internal Byte bState;
			internal Byte bRunning;
			internal Byte bPriority;
			internal Byte bBasePriority;
			internal UInt64 vaETHREAD;
			internal UInt64 vaTeb;
			internal UInt64 ftCreateTime;
			internal UInt64 ftExitTime;
			internal UInt64 vaStartAddress;
			internal UInt64 vaStackBaseUser; // value from _NT_TIB / _TEB
			internal UInt64 vaStackLimitUser; // value from _NT_TIB / _TEB
			internal UInt64 vaStackBaseKernel;
			internal UInt64 vaStackLimitKernel;
			internal UInt64 vaTrapFrame;
			internal UInt64 vaRIP; // RIP register (if user mode)
			internal UInt64 vaRSP; // RSP register (if user mode)
			internal UInt64 qwAffinity;
			internal UInt32 dwUserTime;
			internal UInt32 dwKernelTime;
			internal Byte bSuspendCount;
			internal Byte bWaitReason;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			internal Byte[] _FutureUse1;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
			internal UInt32[] _FutureUse2;

			internal UInt64 vaImpersonationToken;
			internal UInt64 vaWin32StartAddress;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_THREAD {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			internal UInt32[] _Reserved1;

			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetHandle

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HANDLEENTRY {
			internal UInt64 vaObject;
			internal UInt32 dwHandle;
			internal UInt32 dwGrantedAccess_iType;
			internal UInt64 qwHandleCount;
			internal UInt64 qwPointerCount;
			internal UInt64 vaObjectCreateInfo;
			internal UInt64 vaSecurityDescriptor;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt32 _FutureUse2;
			internal UInt32 dwPID;
			internal UInt32 dwPoolTag;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
			internal UInt32[] _FutureUse;

			[MarshalAs(UnmanagedType.LPWStr)] internal String wszType;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_HANDLE {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetNet

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_NETENTRY {
			internal UInt32 dwPID;
			internal UInt32 dwState;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal UInt16[] _FutureUse3;

			internal UInt16 AF;

			// src
			internal Boolean src_fValid;
			internal UInt16 src__Reserved1;
			internal UInt16 src_port;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			internal Byte[] src_pbAddr;

			[MarshalAs(UnmanagedType.LPWStr)] internal String src_wszText;

			// dst
			internal Boolean dst_fValid;
			internal UInt16 dst__Reserved1;
			internal UInt16 dst_port;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			internal Byte[] dst_pbAddr;

			[MarshalAs(UnmanagedType.LPWStr)] internal String dst_wszText;

			//
			internal UInt64 vaObj;
			internal UInt64 ftTime;
			internal UInt32 dwPoolTag;
			internal UInt32 _FutureUse4;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			internal UInt32[] _FutureUse2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_NET {
			internal UInt32 dwVersion;
			internal UInt32 _Reserved1;
			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetPhysMem

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PHYSMEMENTRY {
			internal UInt64 pa;
			internal UInt64 cb;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PHYSMEM {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt32 cMap;
			internal UInt32 _Reserved2;
		}


		// VMMDLL_Map_GetPool

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_POOLENTRY {
			internal UInt64 va;
			internal UInt32 dwTag;
			internal Byte _ReservedZero;
			internal Byte fAlloc;
			internal Byte tpPool;
			internal Byte tpSS;
			internal UInt32 cb;
			internal UInt32 _Filler;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_POOL {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			internal UInt32[] _Reserved1;

			internal UInt32 cbTotal;
			internal IntPtr _piTag2Map;
			internal IntPtr _pTag;
			internal UInt32 cTag;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetUsers

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct VMMDLL_MAP_USERENTRY {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			internal UInt32[] _FutureUse1;

			[MarshalAs(UnmanagedType.LPWStr)] internal String wszText;
			internal UInt64 vaRegHive;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszSID;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			internal UInt32[] _FutureUse2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_USER {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetServuces

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_SERVICEENTRY {
			internal UInt64 vaObj;
			internal UInt32 dwOrdinal;

			internal UInt32 dwStartType;

			// SERVICE_STATUS START
			internal UInt32 dwServiceType;
			internal UInt32 dwCurrentState;
			internal UInt32 dwControlsAccepted;
			internal UInt32 dwWin32ExitCode;
			internal UInt32 dwServiceSpecificExitCode;
			internal UInt32 dwCheckPoint;

			internal UInt32 dwWaitHint;

			// SERVICE_STATUS END
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszServiceName;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszDisplayName;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszPath;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszUserTp;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszUserAcct;
			[MarshalAs(UnmanagedType.LPWStr)] internal String wszImagePath;
			internal UInt32 dwPID;
			internal UInt32 _FutureUse1;
			internal UInt64 _FutureUse2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_SERVICE {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt64 pbMultiText;
			internal UInt32 cbMultiText;
			internal UInt32 cMap;
		}


		// VMMDLL_Map_GetPfn

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PFNENTRY {
			internal UInt32 dwPfn;
			internal UInt32 tpExtended;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] dwPfnPte;

			internal UInt64 va;
			internal UInt64 vaPte;
			internal UInt64 OriginalPte;
			internal UInt32 _u3;
			internal UInt64 _u4;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			internal UInt32[] _FutureUse;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MAP_PFN {
			internal UInt32 dwVersion;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			internal UInt32[] _Reserved1;

			internal UInt32 cMap;
			internal UInt32 _Reserved2;
		}


		// REGISTRY FUNCTIONALITY BELOW:

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_REGISTRY_HIVE_INFORMATION {
			internal UInt64 magic;
			internal UInt16 wVersion;
			internal UInt16 wSize;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x34)]
			internal Byte[] _FutureReserved1;

			internal UInt64 vaCMHIVE;
			internal UInt64 vaHBASE_BLOCK;
			internal UInt32 cbLength;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			internal Byte[] uszName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
			internal Byte[] uszNameShort;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			internal Byte[] uszHiveRootPath;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
			internal UInt64[] _FutureReserved;
		}


		// MEMORY SEARCH FUNCTIONALITY BELOW:

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MEM_SEARCH_CONTEXT_SEARCHENTRY {
			internal UInt32 cbAlign;
			internal UInt32 cb;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			internal Byte[] pb;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			internal Byte[] pbSkipMask;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct VMMDLL_MEM_SEARCH_CONTEXT {
			internal UInt32 dwVersion;
			internal UInt32 _Filler01;
			internal UInt32 _Filler02;
			internal Boolean fAbortRequested;
			internal UInt32 cMaxResult;
			internal UInt32 cSearch;

			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 16)]
			internal VMMDLL_MEM_SEARCH_CONTEXT_SEARCHENTRY[] search;

			internal UInt64 vaMin;
			internal UInt64 vaMax;
			internal UInt64 vaCurrent;
			internal UInt32 _Filler2;
			internal UInt32 cResult;
			internal UInt64 cbReadTotal;
			internal IntPtr pvUserPtrOpt;
			internal IntPtr pfnResultOptCB;
			internal UInt64 ReadFlags;
			internal Boolean fForcePTE;
			internal Boolean fForceVAD;
			internal IntPtr pfnFilterOptCB;
		}
	}
}