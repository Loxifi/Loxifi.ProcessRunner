namespace Loxifi.Interfaces
{
    /// <summary>
    /// Defines an interface for an object that can read a single
    /// byte on request from a data source
    /// </summary>
    internal interface IReadByte
    {
        /// <summary>Reads one byte from the data source</summary>
        /// <returns>The byte value, or -1 if EOS</returns>
        int Read();
    }
}