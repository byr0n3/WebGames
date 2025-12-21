using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebGames.Database;
using WebGames.Database.Models;
using WebGames.Models.Options;

namespace WebGames
{
	internal static class Endpoints
	{
		public static async Task ActivateAsync(HttpContext context, IDbContextFactory<WebGamesDbContext> dbFactory, Guid token)
		{
			var db = await dbFactory.CreateDbContextAsync(context.RequestAborted).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var saved = await db.Users
									.Where((u) => u.AccountConfirmationToken == token)
									.ExecuteUpdateAsync(
										static (calls) =>
											calls.SetProperty(static (u) => u.Flags,
															  static (u) => u.Flags | UserFlags.Active | UserFlags.Confirmed)
												 .SetProperty(static (u) => u.AccountConfirmationToken, (Guid?)null),
										context.RequestAborted
									)
									.ConfigureAwait(false);

				if (saved == 1)
				{
					context.Response.Redirect("/sign-in");
				}
				else
				{
					context.Response.StatusCode = StatusCodes.Status404NotFound;
				}
			}
		}

		public static Task GetProfilePictureAsync(HttpContext context, int userId, IOptions<UploadOptions> uploadOptions)
		{
			// @todo Validate user's profile picture visibility setting (public, friends-only, private)

			var path = uploadOptions.Value.GetProfilePicturePath(userId);

			// @todo Return default
			if (!Path.Exists(path))
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				return Task.CompletedTask;
			}

			return context.Response.SendFileAsync(path, context.RequestAborted);
		}
	}
}
