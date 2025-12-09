using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebGames.Database.Models;

namespace WebGames.Database.EntityTypeConfiguration
{
	public sealed class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaim>
	{
		public void Configure(EntityTypeBuilder<UserClaim> builder)
		{
			builder.Property(static (u) => u.Created)
				   .HasDefaultValueSql("now()");
		}
	}
}
