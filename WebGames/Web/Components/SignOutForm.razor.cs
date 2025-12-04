using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebGames.Services;

namespace WebGames.Web.Components
{
	public sealed partial class SignOutForm : ComponentBase
	{
		private const string formName = nameof(SignOutForm);

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		private readonly EditContext editContext = new(0);

		private async Task SignOutAsync()
		{
			await this.Authentication.SignOutAsync();

			this.Navigation.NavigateTo("/", true);
		}
	}
}
