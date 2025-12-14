using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using WebGames.Services;

namespace WebGames.Web.Components
{
	public sealed partial class AccountDisplay : ComponentBase
	{
		private const string signOutFormName = "SignOut";

		[Inject] public required NavigationManager Nav { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<AccountDisplayLocalization> Localizer { get; init; }

		[Parameter] [EditorRequired] public required ClaimsPrincipal User { get; set; }

		private readonly EditContext editContext = new(0);

		private async Task SignOutAsync()
		{
			await this.Authentication.SignOutAsync();

			this.Nav.NavigateTo("/", true);
		}
	}
}
