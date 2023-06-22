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
            CreatePipe(out IntPtr hRead, out IntPtr hWrite, parentInputs);

            return new Pipe(hRead, hWrite, parentInputs);
        }

        /// <summary>Creates the pipe.</summary>
        /// <param name="hRead">The read handle.</param>
        /// <param name="hWrite">The write handle.</param>
        /// <param name="setHandleInformation">If true, parent inputs.</param>
        private static void CreatePipe(out IntPtr hRead, out IntPtr hWrite, bool setHandleInformation)
        {
            SECURITY_ATTRIBUTES lpPipeAttributes = new()
            {
                bInheritHandle = BOOL.TRUE
            };

            lpPipeAttributes.nLength = Marshal.SizeOf(lpPipeAttributes);

            try
            {
                CreatePipeWithSecurityAttributes(out hRead, out hWrite, ref lpPipeAttributes, 0);

                if (setHandleInformation)
                {
                    if (!SetHandleInformation(hWrite, HANDLE_FLAGS.INHERIT, 0))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary>Creates the pipe.</summary>
        /// <param name="hReadPipe">The h read pipe.</param>
        /// <param name="hWritePipe">The h write pipe.</param>
        /// <param name="lpPipeAttributes">The lp pipe attributes.</param>
        /// <param name="nSize">The n size.</param>
        /// <returns>A bool.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);

        /// <summary>Creates the pipe with security attributes.</summary>
        /// <param name="hReadPipe">The h read pipe.</param>
        /// <param name="hWritePipe">The h write pipe.</param>
        /// <param name="lpPipeAttributes">The lp pipe attributes.</param>
        /// <param name="nSize">The n size.</param>
        private static void CreatePipeWithSecurityAttributes(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize)
        {
            if (!CreatePipe(out hReadPipe, out hWritePipe, ref lpPipeAttributes, nSize))
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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);
    }
}