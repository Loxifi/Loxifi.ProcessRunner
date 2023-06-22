using Loxifi.Data;

namespace Loxifi
{
    /// <summary>
    /// A collection of settings defining the process to execute
    /// </summary>
    public class ProcessSettings
    {
        /// <summary>
        /// Event triggered when data is read from StdErr.
        /// </summary>
        public EventHandler<string>? StdErrWrite;

        /// <summary>
        /// Event triggered when a new line is read from StdErr
        /// </summary>
        public EventHandler<string>? StdErrWriteLine;

        /// <summary>
        /// Event triggered when data is read from StdOut.
        /// </summary>
        public EventHandler<string>? StdOutWrite;

        /// <summary>
        /// Event triggered when a newline is read from StdOut.
        /// </summary>
        public EventHandler<string>? StdOutWriteLine;

        //TODO: Move this somewhere that makes more sense
        /// <summary>
        /// Constructs a new instace of this class with the specified file name
        /// </summary>
        /// <param name="fileName">The file or path to execute</param>
        public ProcessSettings(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("'fileName' cannot be null or whitespace.", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
            {
                fileName += ".exe";
            }

            this.FileName = fileName;
        }

        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        public object Arguments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        public ProcessCredentials? Credentials { get; set; }

        /// <summary>
        /// The file or path to execute.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Create the process in a Unicode environment
        /// </summary>
        public bool UnicodeEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the window style.
        /// </summary>
        public WindowStyle WindowStyle { get; set; } = WindowStyle.SW_SHOWNORMAL;

        /// <summary>
        /// Gets or sets the working directory.
        /// Defaults to the current directory
        /// </summary>
        public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory();
    }
}