using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using WebGames.Database;
using WebGames.Database.Models;

namespace WebGames.Web.Components
{
	public sealed partial class SignOutForm : ComponentBase
	{
		private const string formName = nameof(SignOutForm);

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService<User, WebGamesDbContext> Authentication { get; init; }

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		private readonly EditContext editContext = new(0);

		private async Task SignOutAsync()
		{
			await this.Authentication.SignOutAsync(this.HttpContext);

			this.Navigation.NavigateTo("/", true);
		}
	}
}
