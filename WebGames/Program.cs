using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebGames.Web;

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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);

return;

static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
{
	services.AddRazorComponents().AddInteractiveServerComponents();

	services.AddCascadingAuthenticationState()
			.AddAuthentication();

	services.AddAuthorization();
}
