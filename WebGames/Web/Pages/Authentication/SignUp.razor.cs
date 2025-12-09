using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MimeKit;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.EmailTemplates;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class SignUp : ComponentBase
	{
		private const string formName = nameof(SignUp);

		[Inject] public required SmtpService Smtp { get; init; }

		[Inject] public required DbEncryptor Encryptor { get; init; }

		[Inject] public required RendererService Renderer { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required IStringLocalizer<SignUpLocalization> Localizer { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[SupplyParameterFromForm(FormName = SignUp.formName)]
		private SignUpModel Model { get; set; } = new();

		private async Task SignUpAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var token = System.Guid.NewGuid();

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				db.Users.Add(new User
				{
					Username = this.Encryptor.Encrypt(this.Model.Username),
					Email = this.Encryptor.Encrypt(this.Model.Email),
					Password = Hashing.Hash(this.Model.Password),
					AccountConfirmationToken = token,
				});

				var saved = await db.SaveChangesAsync();

				// @todo Show error
				Debug.Assert(saved == 1);
			}

			var html = await this.Renderer.RenderAsync<AccountConfirmation>(new Dictionary<string, object?>(System.StringComparer.Ordinal)
			{
				{ nameof(AccountConfirmation.Token), token },
			});

			await this.Smtp.SendAsync(new SmtpService.SmtpMessageDescriptor
			{
				To = new MailboxAddress(this.Model.Username, this.Model.Email),
				Subject = AccountConfirmationLocalization.Subject,
				HtmlBody = html,
			});

			this.Navigation.NavigateTo($"/sign-in?{nameof(SignIn.SignedUp)}=true", true);
		}
	}
}
