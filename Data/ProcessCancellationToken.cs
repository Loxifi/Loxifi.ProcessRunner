namespace Loxifi.Data
{
    /// <summary>
    /// The standard cancellation token isn't working
    /// for what I need it for so heres a simple implementation
    /// that does
    /// </summary>
    internal class ProcessCancellationToken
    {
        /// <summary>True if cancel has been requested</summary>
        public bool IsCancelled { get; set; }

        /// <summary>Requests the cancellation of a task/process/thread</summary>
        public void Cancel() => this.IsCancelled = true;
    }
}