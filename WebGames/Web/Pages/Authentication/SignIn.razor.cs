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

		protected override void OnInitialized()
		{
			this.context = new EditContext(this.Model);
		}

		private async Task SignInAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var result = await this.Authentication.AuthenticateAsync(this.Model.User, this.Model.Password, this.Model.Persistent);

			if (result is not AuthenticationResult.Success)
			{
				new ValidationMessageStore(this.context)
					.Add(FieldIdentifier.Create(() => this.Model.User), this.Localizer[result.ToString()]);

				this.Model.Password = null;

				return;
			}

			this.Navigation.NavigateTo(this.GetReturnUrl(), true);
		}

		private string GetReturnUrl()
		{
			const string @default = "/";

			if (string.IsNullOrWhiteSpace(this.ReturnUrl) || (this.ReturnUrl[0] != '/'))
			{
				return @default;
			}

			return this.ReturnUrl;
		}
	}
}
