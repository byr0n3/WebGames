using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using WebGames.AspNet;
using WebGames.Database;
using WebGames.Database.Models;
using WebGames.Extensions;
using WebGames.Resources;
using WebGames.Services;

namespace WebGames.Models.Requests
{
	internal sealed class UpdateAccountModel : IValidatableObject
	{
		[Required]
		[MaxLength(User.UsernameMaxLength)]
		[Display(Name = nameof(UpdateAccountModel.Username), ResourceType = typeof(UserLocalization))]
		public string? Username { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(User.EmailMaxLength)]
		[Display(Name = nameof(UpdateAccountModel.Email), ResourceType = typeof(UserLocalization))]
		public string? Email { get; set; }

		[Password(false)]
		[Display(Name = nameof(UpdateAccountModel.Password), ResourceType = typeof(UserLocalization))]
		public string? Password { get; set; }

		[Display(Name = nameof(UpdateAccountModel.PasswordConfirmation), ResourceType = typeof(UserLocalization))]
		public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Username), nameof(this.Email))]
			get => (this.Username is not null) && (this.Email is not null);
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var userId = context.GetRequiredService<AuthenticationService>().User?.GetClaimValue<int>(ClaimType.Id) ?? default;

			Debug.Assert(userId != default);

			var localizer = context.GetRequiredService<IStringLocalizer<UpdateAccountModel>>();
			var db = context.GetRequiredService<IDbContextFactory<WebGamesDbContext>>().CreateDbContext();

			using (db)
			{
				var query = db.Users.Where((u) => u.Id != userId);

				var usernameExists = query.Any((u) => u.Username == this.Username);
				var emailExists = query.Any((u) => u.Email == this.Email);

				if (usernameExists)
				{
					yield return new ValidationResult(localizer["UsernameTaken"], [nameof(SignUpModel.Username)]);
				}

				if (emailExists)
				{
					yield return new ValidationResult(localizer["EmailTaken"], [nameof(SignUpModel.Email)]);
				}
			}
		}
	}
}
