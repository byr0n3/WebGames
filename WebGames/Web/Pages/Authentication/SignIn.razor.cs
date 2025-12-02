using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using WebGames.Database;
using WebGames.Database.Models;
using WebGames.Models.Requests;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class SignIn : ComponentBase
	{
		private const string formName = nameof(SignIn);

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService<User, WebGamesDbContext> Authentication { get; init; }

		[SupplyParameterFromQuery(Name = nameof(SignIn.ReturnUrl))]
		public string? ReturnUrl { get; set; }

		[SupplyParameterFromForm(FormName = SignIn.formName)]
		private SignInModel Model { get; set; } = new();

		private EditContext context = null!;
		private ValidationMessageStore messageStore = null!;

		protected override void OnInitialized()
		{
			this.context = new EditContext(this.Model);
			this.messageStore = new ValidationMessageStore(this.context);
		}

		private async Task SignInAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var result = await this.Authentication.AuthenticateAsync(this.HttpContext,
																	 this.Model.User,
																	 this.Model.Password,
																	 this.Model.Persistent);

			if (result is not AuthenticationResult.Success)
			{
				// @todo Localize
				var error = (result) switch
				{
					AuthenticationResult.InvalidCredentials => "Invalid credentials.",
					AuthenticationResult.MfaRequired        => "MFA is not supported yet.",
					AuthenticationResult.AccountLockedOut   => "Your account has been temporarily locked.",
					_                                       => "Unknown error",
				};

				this.messageStore.Add(FieldIdentifier.Create(() => this.Model.User), error);

				this.Model.Password = null;

				return;
			}

			// @todo Validate & sanitize
			this.Navigation.NavigateTo(this.ReturnUrl ?? "/", true);
		}
	}
}
