using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebGames.AspNet;
using WebGames.Resources;

namespace WebGames.Models.Requests
{
	internal sealed class ResetPasswordModel
	{
		[Required]
		[Password]
		[Display(Name = nameof(ResetPasswordModel.Password), ResourceType = typeof(UserLocalization))]
		public string? Password { get; set; }

		[Required]
		[Display(Name = nameof(ResetPasswordModel.PasswordConfirmation), ResourceType = typeof(UserLocalization))]
		public string? PasswordConfirmation { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Password), nameof(this.PasswordConfirmation))]
			get => (this.Password is not null) && (this.PasswordConfirmation is not null);
		}
	}
}
