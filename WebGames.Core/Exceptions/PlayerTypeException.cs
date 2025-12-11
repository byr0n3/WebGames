using System;
using System.Diagnostics.CodeAnalysis;

namespace WebGames.Core.Exceptions
{
	/// <summary>
	/// Represents an error that occurs when a player of an incorrect type attempts to join a game.
	/// </summary>
	[SuppressMessage("Roslynator", "RCS1194")]
	public sealed class PlayerTypeException : Exception
	{
		/// <inheritdoc/>
		internal PlayerTypeException(Type gameType, Type expectedPlayerType, Type receivedPlayerType) :
			base($"Player of type {receivedPlayerType.Name} tried to join {gameType.Name}, {expectedPlayerType.Name} expected.")
		{
		}
	}
}
