using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebGames.Resources;

namespace WebGames.Models.Requests
{
	internal sealed class ForgotPasswordModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = nameof(ForgotPasswordModel.Email), ResourceType = typeof(UserLocalization))]
		public string? Email { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Email))]
			get => (this.Email is not null);
		}
	}
}
