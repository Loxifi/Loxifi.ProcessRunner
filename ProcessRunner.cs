using Loxifi.Data;
using Loxifi.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Loxifi
{
	/// <summary>Used to exexecute processes</summary>
	public static class ProcessRunner
	{
		internal const int STARTF_USESHOWWINDOW = 1;
		internal const int STARTF_USESTDHANDLES = 256;

		/// <summary>Executes a process with the given command settings</summary>
		/// <param name="settings">The settings.</param>
		/// <returns>A Task with the return code of the process</returns>
		public static async Task<uint> StartAsync(ProcessSettings settings)
		{
			string commandLine = settings != null ? settings.Arguments : throw new ArgumentNullException(nameof(settings));
			string fileName = settings.FileName;

			if (!settings.FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				string exe = AssocQueryString(AssocStr.Executable, Path.GetExtension(settings.FileName));
				commandLine = "\"" + settings.FileName + "\" " + settings.FileName;
				fileName = exe;			}

			if (!File.Exists(fileName))
			{
				fileName = GetFullPath(fileName);
			}

			commandLine = " " + commandLine;

			Pipe inputPipe = PipeService.Create(true);
			Pipe outputPipe = PipeService.Create(false);
			Pipe errorPipe = PipeService.Create(false);

			STARTUPINFO startupInfo = new()
			{
				dwFlags = STARTF_USESHOWWINDOW | STARTF_USESTDHANDLES,
				wShowWindow = (short)settings.WindowStyle,
				cb = Marshal.SizeOf(typeof(STARTUPINFO)),
				hStdInput = inputPipe.ChildPtr,
				hStdOutput = outputPipe.ChildPtr,
				hStdError = errorPipe.ChildPtr
			};

			IntPtr lpEnvironment = IntPtr.Zero;

			PROCESS_CREATION_FLAGS creationFlags = PROCESS_CREATION_FLAGS.CREATE_DEFAULT_ERROR_MODE;

			if (settings.UnicodeEnvironment)
			{
				_ = CreateEnvironmentBlock(out lpEnvironment, IntPtr.Zero, false);
				creationFlags |= PROCESS_CREATION_FLAGS.CREATE_UNICODE_ENVIRONMENT;
			}

			uint num;
			try
			{
				PROCESS_INFORMATION p;
				if (settings.Credentials != null)
				{
					SplitUsername(settings.Credentials.UserName, out string username, out string domain);
					if (!CreateProcessWithLogonW(username, domain, settings.Credentials.Password, (int)settings.Credentials.LogonStyle, fileName, commandLine, creationFlags, lpEnvironment, settings.WorkingDirectory, ref startupInfo, out p))
					{
						throw new Win32Exception(Marshal.GetLastWin32Error());
					}
				}
				else if (!CreateProcess($"{fileName}", $"\"{fileName}\" {commandLine}", new SECURITY_ATTRIBUTES(), new SECURITY_ATTRIBUTES(), true, creationFlags, lpEnvironment, settings.WorkingDirectory, ref startupInfo, out p))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				ProcessCancellationToken cancellationToken = new();

				StreamDelegate outSd = new(settings.StdOutWrite, settings.StdOutWriteLine);
				Task readOutput = new AsyncStreamReader(outputPipe.Reader!, new Action<string>(outSd.Write), new Action(outputPipe.Dispose)).TryReadAsync(cancellationToken);

				StreamDelegate errSd = new(settings.StdErrWrite, settings.StdErrWriteLine);
				Task readError = new AsyncStreamReader(errorPipe.Reader!, new Action<string>(errSd.Write), new Action(errorPipe.Dispose)).TryReadAsync(cancellationToken);

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
				cancellationToken.Cancel();
				await readOutput;
				await readError;
				_ = CloseHandle(p.hProcess);
				_ = CloseHandle(p.hThread);
				num = toReturn;
			}
			finally
			{
				if (settings.UnicodeEnvironment)
				{
					_ = DestroyEnvironmentBlock(lpEnvironment);
				}
			}

			return num;
		}

		/// <summary>Assocs the query string.</summary>
		/// <param name="association">The association.</param>
		/// <param name="extension">The extension.</param>
		/// <returns>A string.</returns>
		internal static string AssocQueryString(AssocStr association, string extension)
		{
			uint pcchOut = 0;
			if (AssocQueryString(AssocF.None, association, extension, null, null, ref pcchOut) != 1U)
			{
				throw new InvalidOperationException("Could not determine associated string");
			}

			StringBuilder pszOut = new((int)pcchOut);
			if (AssocQueryString(AssocF.None, association, extension, null, pszOut, ref pcchOut) == 0U)
			{
				return pszOut.ToString();
			}

			throw new InvalidOperationException("Could not determine associated string");
		}

		/// <summary>Gets the shell open.</summary>
		/// <param name="path">The path.</param>
		/// <returns>A string.</returns>
		internal static string GetShellOpen(string path) => RegistryService.GetRequiredValue("HKEY_CLASSES_ROOT\\" + RegistryService.GetRequiredValue("HKEY_CLASSES_ROOT\\" + Path.GetExtension(path)) + "\\Shell\\Open\\Command").Replace("%1", path);

		/// <summary>Assocs the query string.</summary>
		/// <param name="flags">The flags.</param>
		/// <param name="str">The str.</param>
		/// <param name="pszAssoc">The psz assoc.</param>
		/// <param name="pszExtra">The psz extra.</param>
		/// <param name="pszOut">The psz out.</param>
		/// <param name="pcchOut">The pcch out.</param>
		/// <returns>An uint.</returns>
		[DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
		private static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string? pszExtra, [Out] StringBuilder? pszOut, ref uint pcchOut);

		/// <summary>Closes the handle.</summary>
		/// <param name="hObject">The h object.</param>
		/// <returns>A bool.</returns>
		[DllImport("kernel32")]
		private static extern bool CloseHandle(IntPtr hObject);

		/// <summary>Creates the environment block.</summary>
		/// <param name="lpEnvironment">The lp environment.</param>
		/// <param name="hToken">The h token.</param>
		/// <param name="bInherit">If true, b inherit.</param>
		/// <returns>A bool.</returns>
		[DllImport("userenv")]
		private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

		/// <summary>Creates the process.</summary>
		/// <param name="applicationName">The application name.</param>
		/// <param name="commandLine">The command line.</param>
		/// <param name="lpProcessAttributes">The lp process attributes.</param>
		/// <param name="lpThreadAttributes">The lp thread attributes.</param>
		/// <param name="bInheritHandles">If true, b inherit handles.</param>
		/// <param name="creationFlags">The creation flags.</param>
		/// <param name="environment">The environment.</param>
		/// <param name="currentDirectory">The current directory.</param>
		/// <param name="lpStartupInfo">The lp startup info.</param>
		/// <param name="lpProcessInformation">The lp process information.</param>
		/// <returns>A bool.</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool CreateProcess(string applicationName, string commandLine, SECURITY_ATTRIBUTES lpProcessAttributes, SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, PROCESS_CREATION_FLAGS creationFlags, IntPtr environment, string currentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

		/// <summary>Creates the process with logon w.</summary>
		/// <param name="userName">The user name.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="password">The password.</param>
		/// <param name="logonFlags">The logon flags.</param>
		/// <param name="applicationName">The application name.</param>
		/// <param name="commandLine">The command line.</param>
		/// <param name="creationFlags">The creation flags.</param>
		/// <param name="environment">The environment.</param>
		/// <param name="currentDirectory">The current directory.</param>
		/// <param name="startupInfo">The startup info.</param>
		/// <param name="processInformation">The process information.</param>
		/// <returns>A bool.</returns>
		[DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool CreateProcessWithLogonW(string userName, string domain, string password, int logonFlags, string applicationName, string commandLine, PROCESS_CREATION_FLAGS creationFlags, IntPtr environment, string currentDirectory, ref STARTUPINFO startupInfo, out PROCESS_INFORMATION processInformation);

		/// <summary>Destroys the environment block.</summary>
		/// <param name="lpEnvironment">The lp environment.</param>
		/// <returns>A bool.</returns>
		[DllImport("userenv")]
		private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

		/// <summary>Gets the exit code process.</summary>
		/// <param name="hProcess">The h process.</param>
		/// <param name="ExitCode">The exit code.</param>
		/// <returns>A bool.</returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetExitCodeProcess(IntPtr hProcess, out uint ExitCode);

		/// <summary>
		/// Attempts to resolve a file name into a full path using
		/// environment variables
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName))
			{
				return Path.GetFullPath(fileName);
			}

			foreach (string path1 in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator))
			{
				string path = Path.Combine(path1, fileName);
				if (File.Exists(path))
				{
					return path;
				}
			}

			throw new FileNotFoundException("Unable to resolve path for file name '" + fileName + "'");
		}

		/// <summary>Splits the username.</summary>
		/// <param name="username">The username.</param>
		/// <param name="user">The user.</param>
		/// <param name="domain">The domain.</param>
		private static void SplitUsername(string username, out string user, out string domain)
		{
			string[] strArray = username.Split(new char[2]
			{
		'\\',
		'@'
			});
			if (strArray.Length == 1)
			{
				user = strArray[0];
				domain = Environment.UserDomainName;
			}
			else if (username.Contains('@'))
			{
				user = strArray[0];
				domain = strArray[1];
			}
			else
			{
				user = strArray[1];
				domain = strArray[0];
			}
		}
	}
}