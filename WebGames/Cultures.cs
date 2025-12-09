using System.Globalization;

namespace WebGames
{
	internal static class Cultures
	{
		public const string CookieName = "WebGames.Culture";

		public static readonly CultureInfo[] Supported =
		[
			new("en-US"),
			new("nl-NL"),
		];

		public static CultureInfo Default =>
			Cultures.Supported[0];
	}
}
