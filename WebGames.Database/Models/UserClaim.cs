using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebGames.Database.EntityTypeConfiguration;

namespace WebGames.Database.Models
{
	[PrimaryKey(nameof(UserClaim.UserId), nameof(UserClaim.Type))]
	[EntityTypeConfiguration(typeof(UserClaimEntityTypeConfiguration))]
	public sealed class UserClaim
	{
		public required int UserId { get; init; }

		[StringLength(128)] public required string Type { get; init; }

		[StringLength(256)] public required string Value { get; init; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime Created { get; init; }

		public User User { get; init; } = null!;
	}
}
