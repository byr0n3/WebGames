using System.Linq;

namespace WebGames.Core.Extensions
{
	public static class GameExtensions
	{
		extension(IGame game)
		{
			/// <summary>
			/// Gets a value indicating whether the game can currently accept additional players.
			/// </summary>
			/// <remarks>
			/// A game is considered joinable when one of the following conditions is true:
			/// <list type="bullet">
			/// <item>The game configuration permits joining after the game has started (<see cref="GameConfiguration.CanJoinInProgress"/> is <see langword="true"/>).</item>
			/// <item>The game is still in the <c>Idle</c> state.</item>
			/// </list>
			/// In addition, the current number of players must be less than the maximum allowed by the game's configuration (<see cref="GameConfiguration.MaxPLayers"/>).
			/// </remarks>
			/// <value>
			/// <see langword="true"/> if the game can accept more players; otherwise, <see langword="false"/>.
			/// </value>
			public bool Joinable =>
				(game.Configuration.CanJoinInProgress || (game.State == GameState.Idle)) &&
				(game.Players.Count < game.Configuration.MaxPLayers);

			/// <summary>
			/// Checks if the specified <see cref="IPlayer"/> is present in the game's player roster.
			/// </summary>
			/// <param name="player">The player whose membership in the game should be verified.</param>
			/// <returns>
			/// <see langword="true"/> if the player is contained in the game's <see cref="IGame.Players"/> collection; otherwise, <see langword="false"/>.
			/// </returns>
			public bool ContainsPlayer(IPlayer player) =>
				game.Players.Contains(player);
		}
	}
}
