using Microsoft.Extensions.Options;

namespace WebGames.Models.Options
{
	public sealed class AppOptions : IOptions<AppOptions>
	{
		public string Name { get; set; } = nameof(WebGames);

		public string BaseUrl { get; set; } = "https://localhost:5001/";

		AppOptions IOptions<AppOptions>.Value =>
			this;
	}
}
