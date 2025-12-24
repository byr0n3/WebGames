using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WebGames.Core;
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

			services.AddDatabase(configuration, true);

			// We want a new GameManager everytime we require it from the service provider,
			// as multithreaded tests can influence the test results.
			services.AddTransient<GameManager>();

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
