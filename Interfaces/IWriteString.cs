namespace Loxifi.Interfaces
{
    /// <summary>
    /// defines an interface for an object that can write
    /// a string to an output stream
    /// </summary>
    internal interface IWriteString
    {
        /// <summary>
        /// Writes a string to the underlying output
        /// stream
        /// </summary>
        /// <param name="toWrite"></param>
        void Write(string toWrite);
    }
}