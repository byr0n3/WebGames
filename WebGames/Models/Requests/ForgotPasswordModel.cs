using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebGames.Models.Requests
{
	internal sealed class ForgotPasswordModel
	{
		[Required] [EmailAddress] public string? Email { get; set; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Email))]
			get => (this.Email is not null);
		}
	}
}
