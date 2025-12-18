using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MimeKit;
using WebGames.Database;
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

		[Inject] public required RendererService Renderer { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Inject] public required IStringLocalizer<ForgotPasswordLocalization> Localizer { get; init; }

		[SupplyParameterFromForm(FormName = ForgotPassword.formName)]
		private ForgotPasswordModel Model { get; set; } = new();

		private bool done;

		private async Task SendAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var token = Guid.NewGuid();
			var expiry = DateTimeOffset.UtcNow.AddMinutes(30);

			UserEmailData? user;

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var saved = await db.Users
									.Where((u) => ((u.Flags & UserFlags.Active) != UserFlags.None) && (u.Email == this.Model.Email))
									.ExecuteUpdateAsync((calls) => calls.SetProperty(static (u) => u.PasswordResetToken, token)
																		.SetProperty(static (u) => u.PasswordResetExpiry, expiry));

				Debug.Assert(saved == 1);

				user = await db.Users
							   .Where((u) => u.PasswordResetToken == token)
							   .Select(static (u) => new UserEmailData
							   {
								   Username = u.Username,
								   Email = u.Email,
							   })
							   .FirstOrDefaultAsync();
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
			var html = await this.Renderer.RenderAsync<ResetPasswordLink>(new Dictionary<string, object?>(System.StringComparer.Ordinal)
			{
				{ nameof(ResetPasswordLink.Token), token },
			});

			await this.Smtp.SendAsync(new SmtpService.SmtpMessageDescriptor
			{
				To = new MailboxAddress(data.Username, data.Email),
				Subject = ResetPasswordLinkLocalization.Subject,
				HtmlBody = html,
			});
		}

		private readonly struct UserEmailData
		{
			public required string Username { get; init; }

			public required string Email { get; init; }
		}
	}
}
