using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using WebGames.Database;
using WebGames.Database.Replication;
using WebGames.Extensions;

namespace WebGames.Web.Components.User
{
	public sealed partial class UserValuesDisplay : ComponentBase, IDisposable
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Inject] public required IDbContextFactory<WebGamesDbContext> DbFactory { get; init; }

		[Parameter] [EditorRequired] public required int UserId { get; set; }

		private ReplicationSubscription? subscription;

		private long coins;
		private int level;

		protected override void OnInitialized()
		{
			this.subscription = DatabaseReplicationService.Subscribe<Database.Models.User>(this.OnUserUpdated);
		}

		protected override Task OnParametersSetAsync() =>
			this.UpdateAsync();

		private void OnUserUpdated(Database.Models.User user, ReplicationType type)
		{
			if ((type != ReplicationType.Updated) || (user.Id != this.UserId))
			{
				return;
			}

			var nextLevel = user.Xp.XpToLevel();
			var updated = (this.coins != user.Coins) || (this.level != nextLevel);

			if (this.coins != user.Coins)
			{
				_ = user.Coins > this.coins
					? this.InvokeAsync(this.PlayCoinsGainedAnimationAsync)
					: this.InvokeAsync(this.PlayCoinsRemovedAnimationAsync);

				this.coins = user.Coins;
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

		private Task PlayCoinsRemovedAnimationAsync() =>
			Task.CompletedTask;

		private Task PlayCoinsGainedAnimationAsync() =>
			Task.CompletedTask;

		private async Task PlayLevelUpAnimationAsync()
		{
			await this.Js.ConfettiAsync(in ConfettiConfig.Default);

			// @todo Show toast/notification
		}

		public void Dispose()
		{
			if (this.subscription is not null)
			{
				DatabaseReplicationService.Unsubscribe(this.subscription);
			}
		}
	}
}
