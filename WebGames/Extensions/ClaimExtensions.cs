using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Elegance.AspNet.Authentication.Extensions;
using WebGames.Core;
using WebGames.Models;

namespace WebGames.Extensions
{
	internal static class ClaimExtensions
	{
		extension(Claim claim)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Claim FromClaimType(ClaimType type, string value) =>
				new(ClaimTypeEnumData.GetValue(type), value);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Claim FromClaimType<T>(ClaimType type, T value, string? format = null) where T : IFormattable =>
				new(ClaimTypeEnumData.GetValue(type), value.ToString(format, CultureInfo.InvariantCulture));
		}

		extension(ClaimsPrincipal claimsPrincipal)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public string GetRequiredClaimValue(ClaimType claimType) =>
				claimsPrincipal.GetRequiredClaimValue(ClaimTypeEnumData.GetValue(claimType));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetClaimValue(ClaimType claimType, [NotNullWhen(true)] out string? result) =>
				claimsPrincipal.TryGetClaimValue(ClaimTypeEnumData.GetValue(claimType), out result);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetClaimValue<T>(ClaimType claimType, [NotNullWhen(true)] out T? result) where T : ISpanParsable<T> =>
				claimsPrincipal.TryGetClaimValue(ClaimTypeEnumData.GetValue(claimType), out result);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public string? GetClaimValue(ClaimType claimType) =>
				claimsPrincipal.GetClaimValue(ClaimTypeEnumData.GetValue(claimType));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public T? GetClaimValue<T>(ClaimType claimType) where T : ISpanParsable<T> =>
				claimsPrincipal.GetClaimValue<T>(ClaimTypeEnumData.GetValue(claimType));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TPlayer AsPlayer<TPlayer>() where TPlayer : IPlayer, new() =>
				new()
				{
					Id = claimsPrincipal.GetClaimValue<int>(ClaimType.Id),
					DisplayName = claimsPrincipal.GetRequiredClaimValue(ClaimType.Username),
				};
		}
	}
}
