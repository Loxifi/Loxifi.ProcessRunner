using Microsoft.Win32.SafeHandles;
using System.Text;

namespace Loxifi
{
	internal class Pipe : IDisposable
	{
		private bool _disposedValue;

		public Pipe(SafeFileHandle parentHandle, SafeFileHandle childHandle, bool isInput)
		{
			this.ParentHandle = parentHandle;
			this.ChildHandle = childHandle;
			this.ChildPtr = childHandle.DangerousGetHandle();
			this.IsInput = isInput;
			if (this.IsInput)
			{
				return;
			}

			this.Reader = new StreamReader(new FileStream(this.ParentHandle, FileAccess.Read, 4096, false), Encoding.UTF8, true, 4096);
		}

		public SafeFileHandle ChildHandle { get; }

		public IntPtr ChildPtr { get; }

		public bool IsInput { get; }

		public SafeFileHandle ParentHandle { get; }

		public StreamReader? Reader { get; }

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
				this.ParentHandle.Dispose();
				this.ChildHandle.Dispose();
			}

			this._disposedValue = true;
		}
	}
}