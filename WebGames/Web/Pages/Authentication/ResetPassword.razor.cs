using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebGames.Database;
using WebGames.Database.Models;
using WebGames.Models.Requests;

namespace WebGames.Web.Pages.Authentication
{
	public sealed partial class ResetPassword : ComponentBase
	{
		private const string formName = nameof(ResetPassword);

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Inject] public required IStringLocalizer<ResetPasswordLocalization> Localizer { get; init; }

		[Parameter] public Guid Token { get; set; }

		[SupplyParameterFromForm(FormName = ResetPassword.formName)]
		private ResetPasswordModel Model { get; set; } = new();

		private EditContext context = null!;

		protected override Task OnInitializedAsync()
		{
			this.context = new EditContext(this.Model);

			return this.ValidateAsync();
		}

		private async Task ValidateAsync()
		{
			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var exists = await this.FindUser(db).AnyAsync();

				if (!exists)
				{
					this.Navigation.NotFound();
				}
			}
		}

		private async Task ResetAsync()
		{
			Debug.Assert(this.Model.IsValid);

			await using (var db = await this.DbFactory.CreateDbContextAsync())
			{
				var saved = await this.FindUser(db)
									  .ExecuteUpdateAsync((calls) =>
															  calls.SetProperty(
																	   static (u) => u.Password,
																	   Hashing.Hash(this.Model.Password)
																   )
																   .SetProperty(
																	   static (u) => u.PasswordResetToken,
																	   default(Guid?)
																   )
																   .SetProperty(
																	   static (u) => u.PasswordResetExpiry,
																	   default(DateTimeOffset?)
																   )
									  );

				if (saved == 1)
				{
					this.Navigation.NavigateTo("/sign-in");
				}
				else
				{
					var messageStore = new ValidationMessageStore(this.context);
					messageStore.Add(FieldIdentifier.Create(() => this.Model.Password), this.Localizer["Error"]);
				}
			}
		}

		private IQueryable<User> FindUser(WebGamesDbContext db) =>
			db.Users.Where((u) => (u.PasswordResetToken == this.Token) && (u.PasswordResetExpiry > DateTimeOffset.UtcNow));
	}
}
