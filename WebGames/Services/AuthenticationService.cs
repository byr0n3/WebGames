using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Http;
using WebGames.Database;
using WebGames.Database.Models;

namespace WebGames.Services
{
	/// <summary>
	/// Handles storing the currently authenticated user and invokes an event to update any dependency that requires the current authentication state.
	/// </summary>
	public sealed class AuthenticationService
	{
		public delegate void OnUserChanged(ClaimsPrincipal? user);

		public event OnUserChanged? UserChanged;

		public ClaimsPrincipal? User
		{
			get;
			set
			{
				field = value;
				this.UserChanged?.Invoke(value);
			}
		}

		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly AuthenticationService<User, WebGamesDbContext> authentication;

		public AuthenticationService(IHttpContextAccessor httpContextAccessor,
									 AuthenticationService<User, WebGamesDbContext> authentication)
		{
			this.authentication = authentication;
			this.httpContextAccessor = httpContextAccessor;

			this.User = this.httpContextAccessor.HttpContext?.User;
		}

		public async Task<AuthenticationResult> AuthenticateAsync(string user, string password, bool persistent)
		{
			var context = this.httpContextAccessor.HttpContext;

			Debug.Assert(context is not null);

			var result = await this.authentication.AuthenticateAsync(context, user, password, persistent).ConfigureAwait(false);

			if (result == AuthenticationResult.Success)
			{
				this.User = context.User;
			}

			return result;
		}

		public Task SignInAsync(User user, bool persistent)
		{
			var context = this.httpContextAccessor.HttpContext;

			Debug.Assert(context is not null);

			return this.authentication.SignInAsync(context, user, persistent);
		}

		public Task SignOutAsync()
		{
			var context = this.httpContextAccessor.HttpContext;

			Debug.Assert(context is not null);

			this.User = null;

			return this.authentication.SignOutAsync(context);
		}
	}
}
