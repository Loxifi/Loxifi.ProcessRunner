// Decompiled with JetBrains decompiler
// Type: Loxifi.AsyncStreamReader
// Assembly: Loxifi.ProcessRunner, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7CEEB837-53CD-46EB-95E6-4BB4BE662D8B
// Assembly location: C:\Users\Loffr\AppData\Local\Temp\Kotimid\d3b99ce600\lib\netstandard2.1\Loxifi.ProcessRunner.dll
// XML documentation location: C:\Users\Loffr\AppData\Local\Temp\Kotimid\d3b99ce600\lib\netstandard2.1\Loxifi.ProcessRunner.xml
using Loxifi.Data;
using Loxifi.Implementations;
using Loxifi.Interfaces;
using System.Text;

namespace Loxifi
{
	/// <summary>
	/// Takes an underlying byte-by-byte reader and wraps it
	/// to produce a readable character stream
	/// </summary>
	internal class AsyncStreamReader
	{
		private readonly IReadByte _byteSource;
		private readonly Action? _close;
		private readonly IWriteString _stringWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Loxifi.AsyncStreamReader" /> class.
		/// </summary>
		/// <param name="byteSource">The byte source.</param>
		/// <param name="stringWriter">A string writer to return read characters on</param>
		public AsyncStreamReader(IReadByte byteSource, IWriteString stringWriter)
		{
			_byteSource = byteSource;
			_stringWriter = stringWriter;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Loxifi.AsyncStreamReader" /> class.
		/// </summary>
		/// <param name="stream">The byte source.</param>
		/// <param name="write">A string action to return read characters on</param>
		/// <param name="close">Ac action to call when the read has completed</param>
		public AsyncStreamReader(StreamReader stream, Action<string> write, Action close)
		{
			_byteSource = new ReadByteStream(stream);
			_stringWriter = new WriteStringAction(write);
			_close = close;
		}

		/// <summary>
		/// Reads off the underlying data stream and returns the results byte by byte
		/// </summary>
		/// <param name="token"></param>
		/// <param name="loopTime"></param>
		/// <returns></returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		public async Task TryReadAsync(ProcessCancellationToken token, int loopTime = 50)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			object writeLock = new();
			StringBuilder sb = new();
			ManualResetEvent threadGate = new(false);
			Thread t = new(() =>
			{
				Monitor.Enter(writeLock);
				while (true)
				{
					Monitor.Exit(writeLock);
					int num;
					try
					{
						num = _byteSource.Read();
					}
					catch (ObjectDisposedException)
					{
						_ = threadGate.Set();
						break;
					}

					Monitor.Enter(writeLock);
					if (num != -1)
					{
						_ = sb.Append((char)num);
					}
				}
			});
			t.Start();
			bool lastLoop = false;
			DateTime lastWrite = DateTime.Now;
			while (true)
			{
				await Task.Delay(loopTime);
				try
				{
					Monitor.Enter(writeLock);
					if (sb.Length > 0)
					{
						_stringWriter.Write(sb.ToString());
						_ = sb.Clear();
						lastWrite = DateTime.Now;
						goto label_16;
					}
					else if ((DateTime.Now - lastWrite).TotalMilliseconds > loopTime)
					{
						if (token.IsCancelled)
						{
							if (lastLoop)
							{
								Action close = _close;
								if (close != null)
								{
									close();
									break;
								}

								break;
							}

							lastLoop = true;
						}
					}
				}
				finally
				{
					Monitor.Exit(writeLock);
				}

			label_16:
				;
			}

			_ = threadGate.WaitOne();
			t = null;
		}
	}
}