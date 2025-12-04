using Microsoft.EntityFrameworkCore;
using WebGames.Database.Models;

namespace WebGames.Database
{
	public sealed class WebGamesDbContext : DbContext
	{
		public required DbSet<User> Users { get; init; }

		public WebGamesDbContext(DbContextOptions<WebGamesDbContext> options) : base(options)
		{
		}
	}
}
