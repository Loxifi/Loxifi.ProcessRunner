using Loxifi.Interfaces;

namespace Loxifi.Implementations
{
	/// <summary>
	/// The write string action.
	/// </summary>
	public class WriteStringAction : IWriteString
	{
		private readonly Action<string> _action;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Loxifi.Implementations.WriteStringAction" /> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public WriteStringAction(Action<string> action)
		{
			this._action = action;
		}

		/// <inheritdoc />
		public void Write(string toWrite)
		{
			Action<string> action = this._action;
			if (action == null)
			{
				return;
			}

			action(toWrite);
		}
	}
}