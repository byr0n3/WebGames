using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.Models.Requests;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class ForgotPassword : ComponentBase
	{
		private const string formName = nameof(ForgotPassword);

		[Inject] public required DbEncryptor Encryptor { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[SupplyParameterFromForm(FormName = ForgotPassword.formName)]
		private ForgotPasswordModel Model { get; set; } = new();

		private bool done;

		private async Task SendAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var token = Guid.NewGuid();
			var expiry = DateTimeOffset.UtcNow.AddMinutes(30); // @todo Configurable

			// Store variable outside the `using` scope, as sending an email could take a while.
			// It would be a waste of resources to keep the DbContext open for all that time, while it's not even being used.
			int saved;

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				saved = await db.Users
								.Where((u) => ((u.Flags & UserFlags.Active) != UserFlags.None) &&
											  (u.Email == this.Encryptor.Encrypt(this.Model.Email)) &&
											  ((u.PasswordResetToken == null) || (u.PasswordResetExpiry <= DateTimeOffset.UtcNow)))
								.ExecuteUpdateAsync((calls) => calls.SetProperty(static (u) => u.PasswordResetToken, token)
																	.SetProperty(static (u) => u.PasswordResetExpiry, expiry));
			}

			this.Model.Email = null;

			this.done = true;

			if (saved == 1)
			{
				// @todo Send email
			}
		}
	}
}
