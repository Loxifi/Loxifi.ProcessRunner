using Loxifi.Data;

namespace Loxifi
{
    /// <summary>
    /// Credential settings to be used when executing a process
    /// </summary>
    public class ProcessCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Loxifi.ProcessCredentials" /> class.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="logonStyle">The logon style.</param>
        public ProcessCredentials(string userName, string password, LogonStyle logonStyle = LogonStyle.LOGON_WITH_PROFILE)
        {
            this.UserName = userName;
            this.Password = password;
            this.LogonStyle = logonStyle;
        }

        /// <summary>Gets the logon style.</summary>
        public LogonStyle LogonStyle { get; private set; }

        /// <summary>Gets the password.</summary>
        public string Password { get; private set; }

        /// <summary>Gets the user name.</summary>
        public string UserName { get; private set; }
    }
}