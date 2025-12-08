using System.ComponentModel.DataAnnotations;
using Elegance.AspNet.Authentication;

namespace WebGames.AspNet
{
	[System.AttributeUsage(System.AttributeTargets.Property)]
	internal sealed class PasswordAttribute : ValidationAttribute
	{
		private readonly bool required;

		public PasswordAttribute(bool required = true)
		{
			this.required = required;
		}

		// @todo Localize errors
		protected override ValidationResult? IsValid(object? @object, ValidationContext context)
		{
			if (!this.required && @object is null)
			{
				return null;
			}

			if (@object is not string password)
			{
				return PasswordAttribute.GetValidationResult("Has to be a valid string value", context);
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				return this.required ? PasswordAttribute.GetValidationResult("Has to be a valid string value", context) : null;
			}

			if (!PasswordStrength.ValidateStrength(password))
			{
				return PasswordAttribute.GetValidationResult("Password not strong enough.", context);
			}

			var object2 = context.ObjectType.GetProperty("PasswordConfirmation")?.GetValue(context.ObjectInstance);

			if (object2 is not string passwordConfirmation ||
				!string.Equals(password, passwordConfirmation, System.StringComparison.Ordinal))
			{
				return PasswordAttribute.GetValidationResult("Password confirmation doesn't match.", context);
			}

			return null;
		}

		private static ValidationResult GetValidationResult(string message, ValidationContext context) =>
			new(message, [context.MemberName ?? context.DisplayName]);
	}
}
