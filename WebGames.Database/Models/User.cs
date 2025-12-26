using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Elegance.AspNet.Authentication;
using Microsoft.EntityFrameworkCore;
using WebGames.Database.EntityTypeConfiguration;

namespace WebGames.Database.Models
{
	[Index(nameof(User.Username), IsUnique = true)]
	[Index(nameof(User.Email), IsUnique = true)]
	[EntityTypeConfiguration(typeof(UserEntityTypeConfiguration))]
	public sealed class User : IAuthenticatable<User>
	{
		public const int UsernameMaxLength = 128;
		public const int EmailMaxLength = 256;

		public int Id { get; init; }

		[StringLength(User.UsernameMaxLength)] public required string Username { get; set; }

		[StringLength(User.EmailMaxLength)] public required string Email { get; set; }

		public required byte[] Password { get; set; }

		public UserFlags Flags { get; init; }

		public Guid? AccountConfirmationToken { get; init; }

		public Guid? PasswordResetToken { get; init; }

		public DateTimeOffset? PasswordResetExpiry { get; init; }

		public string? SecurityStamp { get; init; } = Guid.NewGuid().ToString();

		public DateTimeOffset? LastSignInTimestamp { get; set; }

		public int AccessFailedCount { get; init; }

		public DateTimeOffset? AccessLockoutEnd { get; init; }

		public bool HasMfaEnabled { get; init; }

		public long Coins { get; init; } = 500;

		public long Xp { get; init; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTimeOffset Created { get; init; }

		public List<UserClaim> Claims { get; init; } = null!;

		public static Expression<Func<User, bool>> FindAuthenticatable(string user, IServiceProvider _) =>
			(u) => ((u.Flags & UserFlags.Active) != UserFlags.None) && ((u.Username == user) || (u.Email == user));

		public static IQueryable<User> Include(IQueryable<User> queryable) =>
			queryable.Include(static (u) => u.Claims);
	}
}
