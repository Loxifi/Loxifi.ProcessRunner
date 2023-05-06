using Microsoft.Win32.SafeHandles;
using System.Text;

namespace Loxifi
{
	internal class Pipe : IDisposable
	{
		private bool _disposedValue;

		public Pipe(IntPtr hRead, IntPtr hWrite, bool isInput)
		{
			this.ReadFile = new SafeFileHandle(hRead, true);
			this.WriteFile = new SafeFileHandle(hWrite, true);
			this.WritePtr = hWrite;
			this.ReadPtr = hRead;
			this.IsInput = isInput;

			if (this.IsInput)
			{
				Encoding utf8WithoutBom = new UTF8Encoding(false);
				this.Writer = new StreamWriter(new FileStream(this.WriteFile, FileAccess.Write, 4096, false), utf8WithoutBom, 4096, true);
			}
			else
			{
				this.Reader = new StreamReader(new FileStream(this.ReadFile, FileAccess.Read, 4096, false), true);
			}
		}

		public bool IsInput { get; }

		public StreamReader? Reader { get; }

		public SafeFileHandle ReadFile { get; }

		public IntPtr ReadPtr { get; }

		public SafeFileHandle WriteFile { get; }

		public IntPtr WritePtr { get; }

		public StreamWriter? Writer { get; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._disposedValue)
			{
				return;
			}

			if (disposing)
			{
				this.ReadFile.Dispose();
				this.WriteFile.Dispose();
				this.Reader?.Dispose();
				this.Writer?.Dispose();
			}

			this._disposedValue = true;
		}
	}
}