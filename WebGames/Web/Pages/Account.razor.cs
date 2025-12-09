using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Extensions;
using WebGames.Models;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages
{
	public sealed partial class Account : ComponentBase
	{
		private const string formName = nameof(Account);

		[Inject] public required DbEncryptor Encryptor { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Inject] public required IStringLocalizer<AccountLocalization> Localizer { get; init; }

		[SupplyParameterFromForm(FormName = Account.formName)]
		private UpdateAccountModel Model { get; set; } = new();

		private ClaimsPrincipal User =>
			this.Authentication.User ?? new ClaimsPrincipal();

		private EditContext context = null!;
		private ValidationMessageStore messageStore = null!;
		private bool success;

		protected override void OnInitialized()
		{
			if (!this.Model.IsValid)
			{
				this.Model = new UpdateAccountModel
				{
					Username = this.User.GetRequiredClaimValue(ClaimType.Username),
					Email = this.User.GetRequiredClaimValue(ClaimType.Email),
				};
			}

			this.context = new EditContext(this.Model);
			this.messageStore = new ValidationMessageStore(this.context);
		}

		private async Task UpdateAsync()
		{
			Debug.Assert(this.Model.IsValid);

			this.success = false;

			var userId = this.User.GetClaimValue<int>(ClaimType.Id);

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var user = await db.Users.Where((u) => u.Id == userId).AsTracking().FirstOrDefaultAsync();

				Debug.Assert(user is not null);

				user.Username = this.Encryptor.Encrypt(this.Model.Username);
				user.Email = this.Encryptor.Encrypt(this.Model.Email);

				if (!string.IsNullOrWhiteSpace(this.Model.Password))
				{
					user.Password = Hashing.Hash(this.Model.Password);
				}

				var saved = await db.SaveChangesAsync();

				if (saved == 1)
				{
					// @todo `Persistent` from claim
					await this.Authentication.SignInAsync(user, true);

					this.success = true;
				}
				else
				{
					this.messageStore.Add(FieldIdentifier.Create(() => this.Model.Username), this.Localizer["Failed"]);
				}
			}
		}
	}
}
