using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using Elegance.AspNet.Authentication;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.Extensions;
using WebGames.Models;

namespace WebGames.Services
{
	internal sealed class UserClaimsProvider : IClaimsProvider<User>
	{
		private readonly DbEncryptor encryptor;

		public UserClaimsProvider(DbEncryptor encryptor)
		{
			this.encryptor = encryptor;
		}

		public async IAsyncEnumerable<Claim> GetClaimsAsync(User user, [EnumeratorCancellation] CancellationToken token)
		{
			yield return Claim.FromClaimType(ClaimType.Id, user.Id);
			yield return Claim.FromClaimType(ClaimType.Username, this.encryptor.Decrypt(user.Username));
			yield return Claim.FromClaimType(ClaimType.Email, this.encryptor.Decrypt(user.Email));
			yield return Claim.FromClaimType(ClaimType.Created, user.Created);
		}
	}
}
