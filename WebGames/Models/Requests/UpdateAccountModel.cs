using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebGames.AspNet;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;
using WebGames.Extensions;
using WebGames.Services;

namespace WebGames.Models.Requests
{
	internal sealed class UpdateAccountModel : IValidatableObject
	{
		[Required]
		[MaxLength(User.UsernameMaxLength)]
		public string? Username { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(User.EmailMaxLength)]
		public string? Email { get; set; }

		[Password(false)] public string? Password { get; set; }

		public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Username), nameof(this.Email))]
			get => (this.Username is not null) && (this.Email is not null);
		}

		// @todo Localize errors
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			var userId = context.GetRequiredService<AuthenticationService>().User?.GetClaimValue<int>(ClaimType.Id) ?? default;

			Debug.Assert(userId != default);

			var encryptor = context.GetRequiredService<DbEncryptor>();
			var db = context.GetRequiredService<IDbContextFactory<WebGamesDbContext>>().CreateDbContext();

			var encryptedUsername = encryptor.Encrypt(this.Username);
			var encryptedEmail = encryptor.Encrypt(this.Email);

			using (db)
			{
				var query = db.Users.Where((u) => u.Id != userId);

				var usernameExists = query.Any((u) => u.Username == encryptedUsername);
				var emailExists = query.Any((u) => u.Email == encryptedEmail);

				if (usernameExists)
				{
					yield return new ValidationResult("Username is already taken.", [nameof(SignUpModel.Username)]);
				}

				if (emailExists)
				{
					yield return new ValidationResult("E-mail is already taken.", [nameof(SignUpModel.Email)]);
				}
			}
		}
	}
}
