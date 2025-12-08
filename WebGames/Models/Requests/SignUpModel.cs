using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebGames.Database;
using WebGames.Database.Encryption;
using WebGames.Database.Models;

namespace WebGames.Models.Requests
{
	internal sealed class SignUpModel : IValidatableObject
	{
		[Required]
		[MaxLength(User.UsernameMaxLength)]
		public string? Username { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(User.EmailMaxLength)]
		public string? Email { get; set; }

		[Required] public string? Password { get; set; }

		[Required] public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true,
							   nameof(this.Username), nameof(this.Email),
							   nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Username is not null) && (this.Email is not null) &&
				   (this.Password is not null) && (this.PasswordConfirmation is not null);
		}

		// @todo Localize errors
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			if (!PasswordStrength.ValidateStrength(this.Password))
			{
				yield return new ValidationResult("Password not strong enough.", [nameof(SignUpModel.Password)]);
			}

			var encryptor = context.GetRequiredService<DbEncryptor>();
			var db = context.GetRequiredService<IDbContextFactory<WebGamesDbContext>>().CreateDbContext();

			var encryptedUsername = encryptor.Encrypt(this.Username);
			var encryptedEmail = encryptor.Encrypt(this.Email);

			using (db)
			{
				var usernameExists = db.Users.Any((u) => u.Username == encryptedUsername);
				var emailExists = db.Users.Any((u) => u.Email == encryptedEmail);

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
