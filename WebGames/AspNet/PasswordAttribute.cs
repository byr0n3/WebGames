using System.ComponentModel.DataAnnotations;
using Elegance.AspNet.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

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

		protected override ValidationResult? IsValid(object? @object, ValidationContext context)
		{
			if (!this.required && @object is null)
			{
				return null;
			}

			if (@object is not string password)
			{
				return PasswordAttribute.GetValidationResult("String", context);
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				return this.required ? PasswordAttribute.GetValidationResult("String", context) : null;
			}

			if (!PasswordStrength.ValidateStrength(password))
			{
				return PasswordAttribute.GetValidationResult("Strength", context);
			}

			var object2 = context.ObjectType.GetProperty("PasswordConfirmation")?.GetValue(context.ObjectInstance);

			if (object2 is not string passwordConfirmation ||
				!string.Equals(password, passwordConfirmation, System.StringComparison.Ordinal))
			{
				return PasswordAttribute.GetValidationResult("Confirmation", context);
			}

			return null;
		}

		private static ValidationResult GetValidationResult(string messageKey, ValidationContext context) =>
			new(
				context.GetRequiredService<IStringLocalizer<PasswordAttributeLocalization>>()[messageKey],
				[context.MemberName ?? context.DisplayName]
			);
	}
}
