using System;
using ReClassNET.Plugins;

namespace ReDMA {
	public class ReDMAExt : Plugin {
		public override Boolean Initialize(IPluginHost host) {
			host.Process.CoreFunctions.RegisterFunctions("DMA", new DMAFunctions());
			host.Process.CoreFunctions.SetActiveFunctionsProvider("DMA");

			return base.Initialize(host);
		}
	}
}