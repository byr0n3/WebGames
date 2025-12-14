using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using Elegance.AspNet.Authentication;
using WebGames.Database.Models;
using WebGames.Extensions;
using WebGames.Models;

namespace WebGames.Services
{
	internal sealed class UserClaimsProvider : IClaimsProvider<User>
	{
		public async IAsyncEnumerable<Claim> GetClaimsAsync(User user, [EnumeratorCancellation] CancellationToken token)
		{
			yield return Claim.FromClaimType(ClaimType.Id, user.Id);
			yield return Claim.FromClaimType(ClaimType.Username, user.Username);
			yield return Claim.FromClaimType(ClaimType.Email, user.Email);
			yield return Claim.FromClaimType(ClaimType.Created, user.Created);

			foreach (var claim in user.Claims)
			{
				yield return new Claim(claim.Type, claim.Value);
			}
		}
	}
}
