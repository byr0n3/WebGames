using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;
using WebGames.Database.Models;

namespace WebGames.Services
{
	/// <summary>
	/// Handles tracking the current authentication state for (interactive) server components and revalidates the user's <see cref="User.SecurityStamp"/>.
	/// </summary>
	public sealed class ServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
	{
		private static readonly ClaimsPrincipal defaultUser = new();

		protected override System.TimeSpan RevalidationInterval =>
			System.TimeSpan.FromMinutes(15);

		private readonly AuthenticationService authenticationService;

		public ServerAuthenticationStateProvider(ILoggerFactory loggerFactory, AuthenticationService authenticationService) : base(
			loggerFactory)
		{
			this.authenticationService = authenticationService;

			this.OnAuthenticationChanged(this.authenticationService.User);

			this.authenticationService.UserChanged += this.OnAuthenticationChanged;
		}

		private void OnAuthenticationChanged(ClaimsPrincipal? user) =>
			this.SetAuthenticationState(Task.FromResult(new AuthenticationState(user ?? ServerAuthenticationStateProvider.defaultUser)));

		protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState state, CancellationToken token) =>
			// @todo Validate security stamp (just like in middleware)
			Task.FromResult(true);

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.authenticationService.UserChanged -= this.OnAuthenticationChanged;
			}

			base.Dispose(disposing);
		}
	}
}
