using Loxifi.Data;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Loxifi.Services
{
	internal static class PipeService
	{
		public static Pipe Create(bool parentInputs)
		{
			CreatePipe(out SafeFileHandle parentHandle, out SafeFileHandle childHandle, parentInputs);
			return new Pipe(parentHandle, childHandle, parentInputs);
		}

		/// <summary>Creates the pipe.</summary>
		/// <param name="parentHandle">The parent handle.</param>
		/// <param name="childHandle">The child handle.</param>
		/// <param name="parentInputs">If true, parent inputs.</param>
		private static void CreatePipe(out SafeFileHandle parentHandle, out SafeFileHandle childHandle, bool parentInputs)
		{
			SECURITY_ATTRIBUTES lpPipeAttributes = new()
			{
				bInheritHandle = BOOL.TRUE
			};

			SafeFileHandle? hSourceHandle = null;

			try
			{
				if (parentInputs)
				{
					CreatePipeWithSecurityAttributes(out childHandle, out hSourceHandle, ref lpPipeAttributes, 0);
				}
				else
				{
					CreatePipeWithSecurityAttributes(out hSourceHandle, out childHandle, ref lpPipeAttributes, 0);
				}

				IntPtr currentProcess = GetCurrentProcess();

				if (!DuplicateHandle(currentProcess, hSourceHandle, currentProcess, out parentHandle, 0, false, 2))
				{
					throw new Win32Exception();
				}
			}
			finally
			{
				if (hSourceHandle != null && !hSourceHandle.IsInvalid)
				{
					hSourceHandle.Dispose();
				}
			}
		}

		/// <summary>Creates the pipe.</summary>
		/// <param name="hReadPipe">The h read pipe.</param>
		/// <param name="hWritePipe">The h write pipe.</param>
		/// <param name="lpPipeAttributes">The lp pipe attributes.</param>
		/// <param name="nSize">The n size.</param>
		/// <returns>A bool.</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

		/// <summary>Creates the pipe with security attributes.</summary>
		/// <param name="hReadPipe">The h read pipe.</param>
		/// <param name="hWritePipe">The h write pipe.</param>
		/// <param name="lpPipeAttributes">The lp pipe attributes.</param>
		/// <param name="nSize">The n size.</param>
		private static void CreatePipeWithSecurityAttributes(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize)
		{
			if (!CreatePipe(out hReadPipe, out hWritePipe, ref lpPipeAttributes, nSize) || hReadPipe.IsInvalid || hWritePipe.IsInvalid)
			{
				throw new Win32Exception();
			}
		}

		/// <summary>Duplicates the handle.</summary>
		/// <param name="hSourceProcessHandle">The h source process handle.</param>
		/// <param name="hSourceHandle">The h source handle.</param>
		/// <param name="hTargetProcess">The h target process.</param>
		/// <param name="targetHandle">The target handle.</param>
		/// <param name="dwDesiredAccess">The dw desired access.</param>
		/// <param name="bInheritHandle">If true, b inherit handle.</param>
		/// <param name="dwOptions">The dw options.</param>
		/// <returns>A bool.</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeHandle hSourceHandle, IntPtr hTargetProcess, out SafeFileHandle targetHandle, int dwDesiredAccess, bool bInheritHandle, int dwOptions);

		/// <summary>Gets the current process.</summary>
		/// <returns>An IntPtr.</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();
	}
}