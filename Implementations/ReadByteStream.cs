using Loxifi.Interfaces;

namespace Loxifi.Implementations
{
    /// <summary>
    /// An implementation of an IReadByte
    /// that reads bytes off a streamreader
    /// </summary>
    public class ReadByteStream : IReadByte
    {
        private readonly StreamReader _baseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Loxifi.Implementations.ReadByteStream" /> class.
        /// </summary>
        /// <param name="baseStream">The base stream.</param>
        public ReadByteStream(StreamReader baseStream)
        {
            this._baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        }

        /// <summary>Reads a single byte off a stream</summary>
        /// <returns>An int representing the byte, or -1 of EOS</returns>
        public int Read()
        {
            char[] buffer;
            while (true)
            {
                buffer = new char[1];
                if (this._baseStream.Read((Span<char>)buffer) != 1)
                {
                    Thread.Sleep(5);
                }
                else
                {
                    break;
                }
            }

            return buffer[0];
        }
    }
}