using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using WebGames.Services;

namespace WebGames.Web.Components
{
	public sealed partial class Navigation : ComponentBase
	{
		private const string signOutFormName = "SignOut";

		[Inject] public required NavigationManager Nav { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<NavigationLocalization> Localizer { get; init; }

		private readonly EditContext editContext = new(0);

		private async Task SignOutAsync()
		{
			await this.Authentication.SignOutAsync();

			this.Nav.NavigateTo("/", true);
		}
	}
}
