using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebGames.Services
{
	public sealed class RendererService : IDisposable, IAsyncDisposable
	{
		private readonly IServiceScope scope;
		private readonly HtmlRenderer renderer;

		public RendererService(IServiceProvider services, ILoggerFactory loggerFactory)
		{
			this.scope = services.CreateScope();

			this.renderer = new HtmlRenderer(this.scope.ServiceProvider, loggerFactory);
		}

		public async ValueTask<string> RenderAsync<TComponent>(IDictionary<string, object?> parameters)
			where TComponent : IComponent =>
			await this.renderer
					  .Dispatcher
					  .InvokeAsync(async () =>
					  {
						  var output = await this.renderer
												 .RenderComponentAsync<TComponent>(ParameterView.FromDictionary(parameters))
												 .ConfigureAwait(false);

						  return output.ToHtmlString();
					  })
					  .ConfigureAwait(false);

		public void Dispose()
		{
			this.renderer.Dispose();
			this.scope.Dispose();
		}

		public ValueTask DisposeAsync()
		{
			this.scope.Dispose();

			return this.renderer.DisposeAsync();
		}
	}
}
