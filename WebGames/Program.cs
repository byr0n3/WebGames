using System;
using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebGames;
using WebGames.Database;
using WebGames.Database.Extensions;
using WebGames.Database.Models;
using WebGames.Models;
using WebGames.Services;
using WebGames.Web;
using WebGames.Web.Pages.Authentication;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
else
{
	app.UseExceptionHandler("/error", true);
}

app.UseStatusCodePagesWithReExecute("/not-found", null, true);

app.MapStaticAssets();

app.UseAuth<User, WebGamesDbContext>();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapGet("/activate-account/{token:guid}", Endpoints.ActivateAsync);

await app.RunAsync().ConfigureAwait(false);

return;

static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
{
	services.Configure<AppOptions>((options) =>
	{
		var app = configuration.GetSection("App");

		options.Name = app[nameof(options.Name)] ?? options.Name;
		options.BaseUrl = app[nameof(options.BaseUrl)] ?? options.BaseUrl;
	});

	services.AddRazorComponents().AddInteractiveServerComponents();

	services.AddHttpContextAccessor();

	services.AddScoped<AuthenticationService>();
	services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

	services.AddAuth<User, WebGamesDbContext, UserClaimsProvider>(
		configureCookie: static (options) =>
		{
			options.Cookie.Name = "WebGames.Cookie";

			options.ReturnUrlParameter = nameof(SignIn.ReturnUrl);
			options.AccessDeniedPath = "/not-found";
			options.LoginPath = "/sign-in";
			options.LogoutPath = "/";
		},
		configureAuthentication: static (options) =>
		{
			// @todo MFA support
			options.EnableMfa = false;
		}
	);

	services.AddDatabase(configuration.GetConnectionString("WebGames") ?? throw new Exception(), environment.IsDevelopment());
	services.AddDatabaseEncryptor((options) =>
	{
		var encryption = configuration.GetSection("Encryption");
		var key = encryption[nameof(options.Key)];

		if (!string.IsNullOrWhiteSpace(key))
		{
			options.Key = Convert.FromBase64String(key);
		}
	});

	services.AddSingleton<RendererService>();

	services.Configure<SmtpOptions>((options) =>
	{
		var smtp = configuration.GetSection("Smtp");

		options.Host = smtp[nameof(options.Host)];
		options.Port = smtp.GetValue<int>(nameof(options.Port));
		options.Username = smtp[nameof(options.Username)];
		options.Password = smtp[nameof(options.Password)];
		options.UseSsl = smtp.GetValue<bool>(nameof(options.UseSsl));
		options.ValidateSslCertificate = smtp.GetValue<bool>(nameof(options.ValidateSslCertificate));
		options.DefaultFrom = smtp[nameof(options.DefaultFrom)] ?? options.DefaultFrom;
	});

	services.AddSingleton<SmtpService>();
}
