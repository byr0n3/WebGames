using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using WebGames.Database.Replication;
using WebGames.Database.Models.Options;

namespace WebGames.Database.Extensions
{
	public static class ServiceCollectionExtensions
	{
		extension(IServiceCollection services)
		{
			public void AddDatabase(IConfigurationRoot configuration, bool isDevelopment, int poolSize = 8)
			{
				var connectionString = configuration.GetConnectionString("WebGames") ?? throw new Exception();

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

				services.Configure<DatabaseSynchronizationOptions>((options) =>
				{
					var sync = configuration.GetSection("DatabaseSynchronization");

					options.ConnectionString = sync[nameof(options.ConnectionString)] ?? connectionString;
					options.Slot = sync[nameof(options.Slot)] ?? options.Slot;
				});

				services.AddHostedService<DatabaseReplicationService>();
			}
		}
	}
}
