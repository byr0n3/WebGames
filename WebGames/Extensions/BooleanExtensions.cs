namespace WebGames.Extensions
{
	internal static class BooleanExtensions
	{
		extension(bool value)
		{
			public string Str() =>
				value ? "true" : "false";
		}
	}
}
