using System;
using Elegance.AspNet.Authentication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebGames.Database;
using WebGames.Database.Extensions;
using WebGames.Database.Models;
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

await app.RunAsync().ConfigureAwait(false);

return;

static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
{
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
}
