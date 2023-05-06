using System.Runtime.InteropServices;

namespace Loxifi.Data
{
	/// <summary>
	/// SECURITY_ATTRIBUTES
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SECURITY_ATTRIBUTES
	{
		/// <summary>
		/// nLength
		/// </summary>
		public int nLength;

		/// <summary>
		/// lpSecurityDescriptor
		/// </summary>
		public IntPtr lpSecurityDescriptor;

		/// <summary>
		/// bInheritHandle
		/// </summary>
		public BOOL bInheritHandle;
	}
}