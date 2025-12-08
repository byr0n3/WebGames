using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebGames.Database.Encryption;

namespace WebGames.Database.Models
{
	[Index(nameof(User.Username), IsUnique = true)]
	[Index(nameof(User.Email), IsUnique = true)]
	public sealed class User : IAuthenticatable<User>
	{
		public const int UsernameMaxLength = 128;
		public const int EmailMaxLength = 256;

		public int Id { get; init; }

		public required byte[] Username { get; init; }

		public required byte[] Email { get; init; }

		public required byte[] Password { get; init; }

		public UserFlags Flags { get; init; }

		public Guid? AccountConfirmationToken { get; init; }

		public Guid? PasswordResetToken { get; init; }

		public DateTimeOffset? PasswordResetExpiry { get; init; }

		public string? SecurityStamp { get; init; } = Guid.NewGuid().ToString();

		public DateTimeOffset? LastSignInTimestamp { get; set; }

		public int AccessFailedCount { get; init; }

		public DateTimeOffset? AccessLockoutEnd { get; init; }

		public bool HasMfaEnabled { get; init; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTimeOffset Created { get; init; }

		public static Expression<Func<User, bool>> FindAuthenticatable(string user, IServiceProvider services)
		{
			var encrypted = services.GetRequiredService<DbEncryptor>().Encrypt(user);

			return (u) => ((u.Flags & UserFlags.Active) != UserFlags.None) && ((u.Username == encrypted) || (u.Email == encrypted));
		}
	}
}
