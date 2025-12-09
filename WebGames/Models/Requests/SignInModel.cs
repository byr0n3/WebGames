using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebGames.Resources;

namespace WebGames.Models.Requests
{
	internal sealed class SignInModel
	{
		[Required]
		[Display(Name = nameof(SignInModel.User), ResourceType = typeof(SignInModelLocalization))]
		public string? User { get; set; }

		[Required]
		[Display(Name = nameof(SignInModel.Password), ResourceType = typeof(UserLocalization))]
		public string? Password { get; set; }

		[Display(Name = nameof(SignInModel.Persistent), ResourceType = typeof(SignInModelLocalization))]
		public bool Persistent { get; set; } = true;

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.User), nameof(this.Password))]
			get => (this.User is not null) && (this.Password is not null);
		}
	}
}
