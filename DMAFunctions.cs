using System;
using ReClassNET.Core;
using ReClassNET.Debugger;
using ReClassNET.Memory;
using ReDMA.Vmmsharp;

namespace ReDMA {
	public class DMAFunctions : ICoreProcessFunctions {
		private readonly Object _memDmaLock = new Object();
		private MemDMA _memDma;

		public void EnumerateProcesses(EnumerateProcessCallback callbackProcess) {
			using (MemDMA dma = new MemDMA(autoMemMap: false)) {
				UInt32[] pids = dma.HVmm.PidList();
				foreach (UInt32 pid in pids) {
					Vmm.PROCESS_INFORMATION info = dma.HVmm.ProcessGetInformation(pid);
					EnumerateProcessData data = new EnumerateProcessData {
						Id = (IntPtr)pid, Name = info.szName, Path = info.szNameLong
					};
					callbackProcess(ref data);
				}
			}
		}

		public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection,
			EnumerateRemoteModuleCallback callbackModule) {
			lock (this._memDmaLock) {
				foreach (Vmm.MAP_MODULEENTRY module in this._memDma.HVmm.Map_GetModule((UInt32)process, false)) {
					EnumerateRemoteModuleData moduleData = new EnumerateRemoteModuleData {
						Path = module.wszFullName,
						Size = (IntPtr)module.cbImageSize,
						BaseAddress = (IntPtr)module.vaBase
					};

					callbackModule(ref moduleData);
					foreach (Vmm.IMAGE_SECTION_HEADER section in this._memDma.HVmm.ProcessGetSections((UInt32)process,
						module.wszText)) {
						EnumerateRemoteSectionData sectionData = new EnumerateRemoteSectionData {
							Size = (IntPtr)section.SizeOfRawData,
							Name = section.Name,
							BaseAddress = (IntPtr)(module.vaBase + section.VirtualAddress),
							Category = SectionCategory.HEAP,
							Type = SectionType.Image,
							Protection = SectionProtection.Read | SectionProtection.Write,
							ModulePath = module.wszFullName
						};

						callbackSection(ref sectionData);
					}
				}
			}
		}

		public IntPtr OpenRemoteProcess(IntPtr pid, ProcessAccess desiredAccess) {
			lock (this._memDmaLock) {
				this._memDma?.Dispose();
				this._memDma = new MemDMA { Pid = (UInt32)pid };
			}

			return pid;
		}

		public Boolean IsProcessValid(IntPtr process) {
			lock (this._memDmaLock) {
				return this._memDma != null && this._memDma.IsValid();
			}
		}

		public void CloseRemoteProcess(IntPtr process) {
			lock (this._memDmaLock) {
				if (this._memDma == null) {
					return;
				}

				if (this._memDma.Pid != (UInt64)process) {
					return;
				}

				this._memDma.Dispose();
				this._memDma = null;
			}
		}

		public Boolean ReadRemoteMemory(IntPtr process, IntPtr address, ref Byte[] buffer, Int32 offset, Int32 size) {
			try {
				lock (this._memDmaLock) {
					buffer = this._memDma.ReadBuffer((UInt64)(address + offset), size);
				}

				return buffer != null;
			}
			catch (Exception) {
				return false;
			}
		}

		public Boolean WriteRemoteMemory(IntPtr process, IntPtr address, ref Byte[] buffer, Int32 offset, Int32 size) {
			try {
				lock (this._memDmaLock) {
					return this._memDma.HVmm.MemWrite((UInt32)process, (UInt64)(address + offset), buffer);
				}
			}
			catch (Exception) {
				return false;
			}
		}

		public void ControlRemoteProcess(IntPtr process, ControlRemoteProcessAction action) {
		}

		public Boolean AttachDebuggerToProcess(IntPtr id) {
			return false;
		}

		public void DetachDebuggerFromProcess(IntPtr id) {
		}

		public Boolean AwaitDebugEvent(ref DebugEvent evt, Int32 timeoutInMilliseconds) {
			return false;
		}

		public void HandleDebugEvent(ref DebugEvent evt) {
		}

		public Boolean SetHardwareBreakpoint(IntPtr id, IntPtr address, HardwareBreakpointRegister register,
			HardwareBreakpointTrigger trigger, HardwareBreakpointSize size, Boolean set) {
			return false;
		}
	}
}