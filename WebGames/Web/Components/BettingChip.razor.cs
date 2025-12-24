using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using WebGames.Extensions;

namespace WebGames.Web.Components
{
	public sealed partial class BettingChip : ComponentBase
	{
		[Inject] public required IStringLocalizer<BettingChipLocalization> Localizer { get; init; }

		[Parameter] [EditorRequired] public long Value { get; set; }

		[Parameter] public string Class { get; set; } = string.Empty;

		[Parameter] public EventCallback<long> Clicked { get; set; }

		private bool Clickable =>
			this.Clicked.HasDelegate;

		private string label = string.Empty;
		private string color = string.Empty;

		protected override void OnParametersSet()
		{
			this.label = this.Value.Humanize();
			this.color = BettingChip.GetColor(this.Value);
		}

		private Task ClickedAsync() =>
			this.Clicked.InvokeAsync(this.Value);

		// @todo Based on algorithm
		private static string GetColor(long value) =>
			(value) switch
			{
				>= 10000000 => "#d1a204",
				>= 1000000  => "#379a8b",
				>= 100000   => "#3e9a37",
				>= 10000    => "#39379a",
				>= 1000     => "#8e1099",
				>= 100      => "#a31c1c",
				>= 10       => "#d16711",
				_           => "#bf2b5f",
			};
	}
}
