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
using WebGames.Resources;

namespace WebGames.Models.Requests
{
	internal sealed class SignUpModel : IValidatableObject
	{
		[Required]
		[MaxLength(User.UsernameMaxLength)]
		[Display(Name = nameof(SignUpModel.Username), ResourceType = typeof(UserLocalization))]
		public string? Username { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(User.EmailMaxLength)]
		[Display(Name = nameof(SignUpModel.Email), ResourceType = typeof(UserLocalization))]
		public string? Email { get; set; }

		[Required]
		[Password]
		[Display(Name = nameof(SignUpModel.Password), ResourceType = typeof(UserLocalization))]
		public string? Password { get; set; }

		[Required]
		[Display(Name = nameof(SignUpModel.PasswordConfirmation), ResourceType = typeof(UserLocalization))]
		public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true,
							   nameof(this.Username), nameof(this.Email),
							   nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Username is not null) && (this.Email is not null) &&
				   (this.Password is not null) && (this.PasswordConfirmation is not null);
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var localizer = context.GetRequiredService<IStringLocalizer<SignUpModel>>();
			var db = context.GetRequiredService<IDbContextFactory<WebGamesDbContext>>().CreateDbContext();

			using (db)
			{
				var usernameExists = db.Users.Any((u) => u.Username == this.Username);
				var emailExists = db.Users.Any((u) => u.Email == this.Email);

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
