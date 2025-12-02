using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using WebGames.Database.Encryption;

namespace WebGames.Database.Extensions
{
	public static class ServiceCollectionExtensions
	{
		extension(IServiceCollection services)
		{
			public void AddDatabase(string connectionString, bool isDevelopment, int poolSize = 8)
			{
				services.AddPooledDbContextFactory<WebGamesDbContext>((options) =>
				{
					var builder = new NpgsqlDataSourceBuilder(connectionString);

					// Enums will get mapped here.

					var dataSource = builder.Build();

					options.EnableSensitiveDataLogging(isDevelopment)
						   .EnableDetailedErrors(isDevelopment)
						   .EnableThreadSafetyChecks(isDevelopment)
						   .UseNpgsql(dataSource);
				}, poolSize);
			}

			public void AddDatabaseEncryptor(System.Action<EncryptionOptions> configure)
			{
				services.Configure(configure);
				services.AddSingleton<DbEncryptor>();
			}
		}
	}
}
