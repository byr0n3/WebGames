using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Elegance.AspNet.Authentication;
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

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			Debug.Assert(this.IsValid);

			if (!PasswordStrength.ValidateStrength(this.Password))
			{
				// @todo Localize
				yield return new ValidationResult("Password not strong enough.", [nameof(SignUpModel.Password)]);
			}
		}
	}
}
