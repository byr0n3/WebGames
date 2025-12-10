using System;

namespace WebGames.Core
{
	public interface ICreatableGame
	{
		/// <summary>
		/// Creates a new game instance identified by the provided <paramref name="code"/> using the supplied <paramref name="configuration"/> and <paramref name="services"/>.
		/// </summary>
		/// <param name="code">The unique code that identifies the game.</param>
		/// <param name="configuration">The <see cref="GameConfiguration"/> that defines the game's settings such as player limits and start behavior.</param>
		/// <param name="services">An <see cref="IServiceProvider"/> used to resolve any required services for the game implementation.</param>
		/// <returns>An <see cref="IGame"/> representing the newly created game.</returns>
		public static abstract IGame Create(string code, GameConfiguration configuration, IServiceProvider services);
	}
}
