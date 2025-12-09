using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebGames.Database.Models;

namespace WebGames.Database.EntityTypeConfiguration
{
	public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.Property(static (u) => u.Created)
				   .HasDefaultValueSql("now()");

			builder.HasMany(static (u) => u.Claims)
				   .WithOne(static (uc) => uc.User);
		}
	}
}
