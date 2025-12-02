using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebGames.Models.Requests
{
	internal sealed class SignInModel
	{
		[Required] public string? User { get; set; }

		[Required] public string? Password { get; set; }

		public bool Persistent { get; set; } = true;

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.User), nameof(this.Password))]
			get => (this.User is not null) && (this.Password is not null);
		}
	}
}
