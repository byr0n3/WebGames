using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace WebGames.Models.Requests
{
	internal sealed class UpdateAccountSettingsModel : IValidatableObject
	{
		[Required]
		[Display(Name = nameof(UpdateAccountSettingsModel.FormatCulture), ResourceType = typeof(UpdateAccountSettingsModelLocalization))]
		public string FormatCulture { get; set; } = CultureInfo.CurrentCulture.Name;

		[Required]
		[Display(Name = nameof(UpdateAccountSettingsModel.UiCulture), ResourceType = typeof(UpdateAccountSettingsModelLocalization))]
		public string UiCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			var localizer = context.GetRequiredService<IStringLocalizer<UpdateAccountSettingsModelLocalization>>();

			if (!Cultures.Supported.Contains(new CultureInfo(this.FormatCulture)))
			{
				yield return new ValidationResult(localizer["FormatCultureError"], [nameof(this.FormatCulture)]);
			}

			if (!Cultures.Supported.Contains(new CultureInfo(this.UiCulture)))
			{
				yield return new ValidationResult(localizer["UiCultureError"], [nameof(this.UiCulture)]);
			}
		}
	}
}
