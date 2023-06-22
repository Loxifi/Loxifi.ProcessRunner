using System.Text;

namespace Loxifi
{
    internal class StreamDelegate : IDisposable
    {
        private readonly StringBuilder? _lineBuffer;

        private readonly EventHandler<string>? _writeHandler;

        private readonly EventHandler<string>? _writeLineHandler;

        private bool _disposedValue;

        public StreamDelegate(EventHandler<string>? writeHandler, EventHandler<string>? writeLineHandler)
        {
            this._writeHandler = writeHandler;
            this._writeLineHandler = writeLineHandler;
            if (this._writeLineHandler == null)
            {
                return;
            }

            this._lineBuffer = new StringBuilder();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Write(string data)
        {
            if (this._writeLineHandler != null && this._lineBuffer != null)
            {
                string[] strArray = data.Split('\n');
                int length = strArray.Length;
                for (int index = 1; index < strArray.Length; ++index)
                {
                    if (index == 0 && length == 1)
                    {
                        _ = this._lineBuffer.Append(strArray[index]);
                    }
                    else if (index == 0 && length > 1)
                    {
                        _ = this._lineBuffer.Append(strArray[index]);
                        this._writeLineHandler(this, this._lineBuffer.ToString().Trim('\r'));
                        _ = this._lineBuffer.Clear();
                    }
                    else if (index > 0 && index < length - 1)
                    {
                        this._writeLineHandler(this, strArray[index].Trim('\r'));
                    }
                    else
                    {
                        if (index <= 0 || index != length - index)
                        {
                            throw new NotImplementedException("Not sure how we got here");
                        }

                        _ = this._lineBuffer.Append(strArray[index]);
                    }
                }
            }

            EventHandler<string> writeHandler = this._writeHandler;
            if (writeHandler == null)
            {
                return;
            }

            writeHandler(this, data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposedValue)
            {
                return;
            }

            if (disposing)
            {
                this._writeLineHandler?.Invoke(this, this._lineBuffer.ToString().Trim('\r'));
                _ = (this._lineBuffer?.Clear());
            }

            this._disposedValue = true;
        }
    }
}