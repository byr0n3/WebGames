namespace WebGames.Core.Events
{
	public readonly struct GameListUpdatedArgs
	{
		public readonly IGame Game;
		public readonly GameListUpdatedType Type;

		public GameListUpdatedArgs(IGame game, GameListUpdatedType type)
		{
			this.Game = game;
			this.Type = type;
		}
	}

	public enum GameListUpdatedType
	{
		Created,
		Updated,
		Removed,
	}
}
