using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace WebGames.AspNet
{
	internal sealed class SynologyHttpsRedirectMiddleware
	{
		private readonly RequestDelegate next;

		public SynologyHttpsRedirectMiddleware(RequestDelegate next) =>
			this.next = next;

		public Task InvokeAsync(HttpContext context)
		{
			var request = context.Request;

			if (!SynologyHttpsRedirectMiddleware.TryUpdateRequestScheme(request) || request.IsHttps)
			{
				return this.next(context);
			}

			var response = context.Response;

			response.StatusCode = StatusCodes.Status308PermanentRedirect;
			response.Headers.Location = UriHelper.BuildAbsolute(
				"https",
				request.Host,
				request.PathBase,
				request.Path,
				request.QueryString
			);

			return Task.CompletedTask;
		}

		private static bool TryUpdateRequestScheme(HttpRequest request)
		{
			const string key = "X-Forwarded-Proto";

			if (!request.Headers.TryGetValue(key, out var value))
			{
				return false;
			}

			if (value.Count != 1)
			{
				return false;
			}

			request.IsHttps = value[0] is "https";

			return true;
		}
	}
}
