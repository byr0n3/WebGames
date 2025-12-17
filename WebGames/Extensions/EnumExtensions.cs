using System;
using WebGames.Utilities;

namespace WebGames.Extensions
{
	internal static class EnumExtensions
	{
		extension<TEnum>(TEnum value) where TEnum : Enum
		{
			public string ToDisplayName() =>
				DisplayName.Get(value);
		}
	}
}
