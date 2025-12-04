using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Elegance.AspNet.Authentication;

namespace WebGames.Models.Requests
{
	internal sealed class ResetPasswordModel : IValidatableObject
	{
		[Required] public string? Password { get; set; }

		[Required] public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Password is not null) && (this.PasswordConfirmation is not null);
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
