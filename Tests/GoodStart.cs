using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Loxifi
{
	internal static class GoodStart
	{
		// P/Invoke declarations
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CreateProcess(string? lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment,
			string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

		[StructLayout(LayoutKind.Sequential)]
		private struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		[Flags]
		private enum HANDLE_FLAGS : uint
		{
			None = 0,
			INHERIT = 1,
			PROTECT_FROM_CLOSE = 2
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct STARTUPINFO
		{
			public int cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		public static void Test()
		{
			SECURITY_ATTRIBUTES saAttr = new();
			saAttr.nLength = Marshal.SizeOf(saAttr);
			saAttr.bInheritHandle = true;
			saAttr.lpSecurityDescriptor = IntPtr.Zero;

			if (!CreatePipe(out nint hRead, out nint hWrite, ref saAttr, 0))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			if (!SetHandleInformation(hWrite, HANDLE_FLAGS.INHERIT, 0))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			STARTUPINFO si = new();
			si.cb = Marshal.SizeOf(si);
			si.dwFlags = 0x00000100; // STARTF_USESTDHANDLES
			si.hStdInput = hRead;
			si.hStdOutput = IntPtr.Zero;
			si.hStdError = IntPtr.Zero;

			if (!CreateProcess(null, "cmd.exe", ref saAttr, ref saAttr, true, 0, IntPtr.Zero, null, ref si, out PROCESS_INFORMATION pi))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			_ = CloseHandle(hRead);

			using (SafeFileHandle sfhWrite = new(hWrite, true))
			{
				using StreamWriter sw = new(new FileStream(sfhWrite, FileAccess.Write), Encoding.UTF8);
				sw.WriteLine("Data to be sent to the child process");
				sw.Flush();
			}

			// Wait for the child process to exit
			_ = WaitForSingleObject(pi.hProcess, INFINITE);

			// Clean up process handles
			_ = CloseHandle(pi.hProcess);
			_ = CloseHandle(pi.hThread);
		}

		// P/Invoke declarations for WaitForSingleObject
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		private const uint INFINITE = 0xFFFFFFFF;
	}
}
