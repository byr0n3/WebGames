using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebGames.Database;
using WebGames.Database.Models;

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
	}
}
