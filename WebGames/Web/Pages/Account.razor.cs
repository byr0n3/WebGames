using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebGames.Database;
using WebGames.Extensions;
using WebGames.Models;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages
{
	public sealed partial class Account : ComponentBase
	{
		private const string settingsFormName = "AccountSettings";
		private const string detailsFormName = "AccountDetails";

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Inject] public required IStringLocalizer<AccountLocalization> Localizer { get; init; }

		[Inject] public required IStringLocalizer<UpdateAccountSettingsModelLocalization> SettingsLocalizer { get; init; }

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[SupplyParameterFromForm(FormName = Account.settingsFormName)]
		private UpdateAccountSettingsModel SettingsModel { get; set; } = new();

		[SupplyParameterFromForm(FormName = Account.detailsFormName)]
		private UpdateAccountDetailsModel DetailsModel { get; set; } = new();

		private ClaimsPrincipal User =>
			this.Authentication.User ?? new ClaimsPrincipal();

		private bool success;

		protected override void OnInitialized()
		{
			if (!this.DetailsModel.IsValid)
			{
				this.DetailsModel = new UpdateAccountDetailsModel
				{
					Username = this.User.GetRequiredClaimValue(ClaimType.Username),
					Email = this.User.GetRequiredClaimValue(ClaimType.Email),
				};
			}
		}

		private void UpdateSettings()
		{
			var requestCulture = new RequestCulture(this.SettingsModel.FormatCulture, this.SettingsModel.UiCulture);

			this.HttpContext.Response.Cookies.Append(Cultures.CookieName, CookieRequestCultureProvider.MakeCookieValue(requestCulture));

			// We need to refresh the page as it started rendering before the culture changed.
			this.Navigation.Refresh();
		}

		private async Task UpdateDetailsAsync()
		{
			Debug.Assert(this.DetailsModel.IsValid);

			this.success = false;

			var userId = this.User.GetClaimValue<int>(ClaimType.Id);

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var user = await db.Users.Where((u) => u.Id == userId).AsTracking().FirstOrDefaultAsync();

				Debug.Assert(user is not null);

				user.Username = this.DetailsModel.Username;
				user.Email = this.DetailsModel.Email;

				if (!string.IsNullOrWhiteSpace(this.DetailsModel.Password))
				{
					user.Password = Hashing.Hash(this.DetailsModel.Password);
				}

				var saved = await db.SaveChangesAsync();

				if (saved == 1)
				{
					// @todo `Persistent` from claim
					await this.Authentication.SignInAsync(user, true);
				}

				this.success = true;
			}
		}
	}
}
