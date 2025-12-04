using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WebGames.Database.Extensions;
using WebGames.Tests;
using Xunit;

[assembly: CaptureConsole]
[assembly: AssemblyFixture(typeof(TestsServiceProvider))]

namespace WebGames.Tests
{
	public sealed class TestsServiceProvider : IServiceProvider, IDisposable, IAsyncDisposable
	{
		private readonly ServiceProvider provider;

		public TestsServiceProvider()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true)
														  .AddJsonFile("appsettings.Development.json", false, true)
														  .AddUserSecrets<TestsServiceProvider>()
														  .Build();

			var services = new ServiceCollection();

			services.AddLogging((builder) =>
			{
				builder.AddConfiguration(configuration.GetSection("Logging"));
				builder.AddSimpleConsole(static (e) =>
				{
					e.ColorBehavior = LoggerColorBehavior.Enabled;
				});
			});

			services.AddDatabase(configuration.GetConnectionString("WebGames") ?? throw new Exception(), true);
			services.AddDatabaseEncryptor((options) =>
			{
				var encryption = configuration.GetSection("Encryption");
				var key = encryption[nameof(options.Key)];

				if (!string.IsNullOrWhiteSpace(key))
				{
					options.Key = Convert.FromBase64String(key);
				}
			});

			this.provider = services.BuildServiceProvider();
		}

		object? System.IServiceProvider.GetService(System.Type type) =>
			this.provider.GetService(type);

		public void Dispose() =>
			this.provider.Dispose();

		public ValueTask DisposeAsync() =>
			this.provider.DisposeAsync();
	}
}
