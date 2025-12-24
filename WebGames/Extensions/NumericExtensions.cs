using System;
using System.Globalization;
using Elegance.Extensions;

namespace WebGames.Extensions
{
	internal static class NumericExtensions
	{
		extension(long value)
		{
			public string Humanize(IFormatProvider? provider = null)
			{
				const float divide = 3f;

				var mag = (int)(float.Floor(float.Log10(value)) / divide);
				var divisor = float.Pow(10f, mag * divide);

				char? suffix = (mag) switch
				{
					3 => 'B',
					2 => 'M',
					1 => 'K',
					_ => null,
				};

				if (suffix is null)
				{
					return value.Str();
				}

				var rounded = float.Round(value / divisor, 1, System.MidpointRounding.ToZero);

				provider ??= CultureInfo.CurrentCulture;

				return string.Create(provider, $"{rounded:N0}{suffix}");
			}

			public int XpToLevel() =>
				1 + (int)float.Floor(0.2f * float.Sqrt(value));
		}
	}
}
