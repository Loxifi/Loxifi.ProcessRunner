using System.Runtime.InteropServices;

namespace Loxifi.Data
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public uint dwProcessId;

		public uint dwThreadId;
	}
}