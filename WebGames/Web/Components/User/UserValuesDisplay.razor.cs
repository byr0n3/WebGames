using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using WebGames.Database;
using WebGames.Database.BackgroundServices;
using WebGames.Extensions;

namespace WebGames.Web.Components.User
{
	public sealed partial class UserValuesDisplay : ComponentBase, IDisposable
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Parameter] [EditorRequired] public required int UserId { get; set; }

		private long coins;
		private int level;

		protected override void OnInitialized()
		{
			DatabaseSynchronizationService.UpdateMessageReceived += this.OnUpdateMessageReceived;
		}

		protected override Task OnParametersSetAsync() =>
			this.UpdateAsync();

		private void OnUpdateMessageReceived(string table, DatabaseUpdateData data)
		{
			if ((table is not "Users") || (data.Get<int>(nameof(Database.Models.User.Id)) != this.UserId))
			{
				return;
			}

			var nextCoins = data.Get<long>(nameof(Database.Models.User.Coins));
			var nextLevel = data.Get<long>(nameof(Database.Models.User.Xp)).XpToLevel();
			var updated = (this.coins != nextCoins) || (this.level != nextLevel);

			if (this.coins != nextCoins)
			{
				this.coins = nextCoins;

				// @todo Animation
			}

			if (this.level != nextLevel)
			{
				this.level = nextLevel;

				_ = this.InvokeAsync(this.PlayLevelUpAnimationAsync);
			}

			if (updated)
			{
				_ = this.InvokeAsync(this.StateHasChanged);
			}
		}

		private async Task UpdateAsync()
		{
			var db = await this.DbFactory.CreateDbContextAsync();

			await using (db)
			{
				var data = await db.Users
								   .Where((u) => u.Id == this.UserId)
								   .Select(static (u) => new
								   {
									   u.Coins,
									   u.Xp,
								   })
								   .FirstOrDefaultAsync();

				if (data is not null)
				{
					this.coins = data.Coins;
					this.level = data.Xp.XpToLevel();
				}
			}
		}

		private async Task PlayLevelUpAnimationAsync()
		{
			await this.Js.ConfettiAsync(in ConfettiConfig.Default);

			// @todo Show toast/notification
		}

		public void Dispose()
		{
			DatabaseSynchronizationService.UpdateMessageReceived -= this.OnUpdateMessageReceived;
		}
	}
}
