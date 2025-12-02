using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.Models.Requests;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class SignUp : ComponentBase
	{
		private const string formName = nameof(SignUp);

		[Inject] public required DbEncryptor Encryptor { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[SupplyParameterFromForm(FormName = SignUp.formName)]
		private SignUpModel Model { get; set; } = new();

		private async Task SignUpAsync()
		{
			Debug.Assert(this.Model.IsValid);

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				db.Users.Add(new User
				{
					Username = this.Encryptor.Encrypt(this.Model.Username),
					Email = this.Encryptor.Encrypt(this.Model.Email),
					Password = Hashing.Hash(this.Model.Password),
				});

				var saved = await db.SaveChangesAsync();

				// @todo Show error
				Debug.Assert(saved == 1);
			}

			// @todo Send welcome email with confirmation URL

			this.Navigation.NavigateTo("/sign-in?sign-up=true", true);
		}
	}
}
