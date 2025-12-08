using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.EmailTemplates;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class ForgotPassword : ComponentBase
	{
		private const string formName = nameof(ForgotPassword);

		[Inject] public required SmtpService Smtp { get; init; }

		[Inject] public required DbEncryptor Encryptor { get; init; }

		[Inject] public required RendererService Renderer { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[SupplyParameterFromForm(FormName = ForgotPassword.formName)]
		private ForgotPasswordModel Model { get; set; } = new();

		private bool done;

		private async Task SendAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var token = Guid.NewGuid();
			var expiry = DateTimeOffset.UtcNow.AddMinutes(30); // @todo Configurable
			UserEmailData? user = null;

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var saved = await db.Users
									.Where((u) => ((u.Flags & UserFlags.Active) != UserFlags.None) &&
												  (u.Email == this.Encryptor.Encrypt(this.Model.Email)))
									.ExecuteUpdateAsync((calls) => calls.SetProperty(static (u) => u.PasswordResetToken, token)
																		.SetProperty(static (u) => u.PasswordResetExpiry, expiry));

				if (saved == 1)
				{
					user = await db.Users
								   .Where((u) => u.PasswordResetToken == token)
								   .Select(static (u) => new UserEmailData
								   {
									   Username = u.Username,
									   Email = u.Email,
								   })
								   .FirstOrDefaultAsync();
				}
				else
				{
					// @todo Show error
				}
			}

			this.Model.Email = null;

			this.done = true;

			if (user is { } data)
			{
				await this.SendEmailAsync(data, token);
			}
		}

		private async Task SendEmailAsync(UserEmailData data, Guid token)
		{
			var username = this.Encryptor.Decrypt(data.Username);
			var email = this.Encryptor.Decrypt(data.Email);

			var html = await this.Renderer.RenderAsync<ResetPasswordLink>(new Dictionary<string, object?>(System.StringComparer.Ordinal)
			{
				{ nameof(ResetPasswordLink.Token), token },
			});

			await this.Smtp.SendAsync(new SmtpService.SmtpMessageDescriptor
			{
				To = new MailboxAddress(username, email),
				Subject = ResetPasswordLink.Subject,
				HtmlBody = html,
			});
		}

		private readonly struct UserEmailData
		{
			public required byte[] Username { get; init; }

			public required byte[] Email { get; init; }
		}
	}
}
