using Loxifi.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Loxifi
{
    /// <summary>
    /// Represents an instance of a running process
    /// </summary>
    public class RunningProcess
    {
        internal EventHandler<string>? StdInWrite;

        internal EventHandler<string>? StdInWriteLine;

        internal RunningProcess(ProcessSettings settings, string commandLine)
        {
            this.Settings = settings;
            this.CommandLine = commandLine;
        }

        public string CommandLine { get; private set; }

        public Task<uint> ExitCode { get; private set; }

        public ProcessSettings Settings { get; }

        public TaskAwaiter<uint> GetAwaiter() => this.ExitCode.GetAwaiter();

        /// <summary>
        /// Invoke me to write to the executing process
        /// </summary>
        public void Write(string toWrite) => this.StdInWrite?.Invoke(this, toWrite);

        /// <summary>
        /// Invoke me to write a new line to the executing process
        /// </summary>
        public void WriteLine(string toWrite) => this.StdInWriteLine?.Invoke(this, toWrite);

        internal void SetProcess(PROCESS_INFORMATION p, Func<Task> cleanup)
        {
            this.ExitCode = Task.Run(async () =>
            {
                try
                {
                    Process process = Process.GetProcessById((int)p.dwProcessId);

                    process.WaitForExit();
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("has exited"))
                {
                }
                catch (ArgumentException ex) when (ex.Message.Contains("is not running"))
                {
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    throw;
                }

                _ = GetExitCodeProcess(p.hProcess, out uint toReturn);

                await cleanup.Invoke();

                return toReturn;
            });
        }

        /// <summary>Gets the exit code process.</summary>
        /// <param name="hProcess">The h process.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <returns>A bool.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetExitCodeProcess(IntPtr hProcess, out uint exitCode);
    }
}