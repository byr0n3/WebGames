using System;
using System.Globalization;
using System.Numerics;

namespace WebGames.Extensions
{
	internal static class NumericExtension
	{
		extension<TNumber>(TNumber value) where TNumber : INumber<TNumber>
		{
			public string Str(string? format = null, IFormatProvider? provider = null)
			{
				provider ??= NumberFormatInfo.InvariantInfo;

				return value.ToString(format, provider);
			}
		}
	}
}
