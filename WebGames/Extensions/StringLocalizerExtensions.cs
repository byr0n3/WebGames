using System;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace WebGames.Extensions
{
	internal static class StringLocalizerExtensions
	{
		extension(IStringLocalizer localizer)
		{
			public string Format(string key, params ReadOnlySpan<object> @params) =>
				string.Format(CultureInfo.CurrentCulture, localizer[key], @params);
		}
	}
}
