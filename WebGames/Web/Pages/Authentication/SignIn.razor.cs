using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using WebGames.Models.Requests;
using WebGames.Services;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class SignIn : ComponentBase
	{
		private const string formName = nameof(SignIn);

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<SignInLocalization> Localizer { get; init; }

		[SupplyParameterFromQuery(Name = nameof(SignIn.ReturnUrl))]
		public string? ReturnUrl { get; set; }

		[SupplyParameterFromQuery(Name = nameof(SignIn.SignedUp))]
		public bool SignedUp { get; set; }

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

			var result = await this.Authentication.AuthenticateAsync(this.Model.User, this.Model.Password, this.Model.Persistent);

			if (result is not AuthenticationResult.Success)
			{
				var error = (result) switch
				{
					AuthenticationResult.InvalidCredentials => nameof(AuthenticationResult.InvalidCredentials),
					AuthenticationResult.MfaRequired        => nameof(AuthenticationResult.MfaRequired),
					AuthenticationResult.AccountLockedOut   => nameof(AuthenticationResult.AccountLockedOut),
					_                                       => nameof(AuthenticationResult.UnknownError),
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
