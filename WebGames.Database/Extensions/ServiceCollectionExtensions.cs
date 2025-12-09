using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

					options.ConfigureWarnings(static (warnings) =>
					{
						// As I like to make manual changes to migration files, I despise this error.
						warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
					});

					options.EnableSensitiveDataLogging(isDevelopment)
						   .EnableDetailedErrors(isDevelopment)
						   .EnableThreadSafetyChecks(isDevelopment)
						   .UseNpgsql(dataSource);
				}, poolSize);
			}

			public void AddDatabaseEncryptor(Action<EncryptionOptions> configure)
			{
				services.Configure(configure);
				services.AddSingleton<DbEncryptor>();
			}
		}
	}
}
