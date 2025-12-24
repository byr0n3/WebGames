using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace WebGames.Database.Models.Options
{
	public sealed class DatabaseSynchronizationOptions : IOptions<DatabaseSynchronizationOptions>
	{
		public const string PublicationName = "webgamespublication";

		public string? ConnectionString { get; set; }

		public string Slot { get; set; } = nameof(WebGames) + "Slot";

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(DatabaseSynchronizationOptions.ConnectionString))]
			get => (this.ConnectionString is not null);
		}

		DatabaseSynchronizationOptions IOptions<DatabaseSynchronizationOptions>.Value =>
			this;
	}
}
