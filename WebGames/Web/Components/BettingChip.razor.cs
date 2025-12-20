using System.Globalization;
using System.Threading.Tasks;
using Elegance.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace WebGames.Web.Components
{
	public sealed partial class BettingChip : ComponentBase
	{
		[Inject] public required IStringLocalizer<BettingChipLocalization> Localizer { get; init; }

		[Parameter] [EditorRequired] public ulong Value { get; set; }

		[Parameter] public string Class { get; set; } = string.Empty;

		[Parameter] public EventCallback<ulong> Clicked { get; set; }

		private bool Clickable =>
			this.Clicked.HasDelegate;

		private string label = string.Empty;
		private string color = string.Empty;

		protected override void OnParametersSet()
		{
			this.label = BettingChip.Humanize(this.Value);
			this.color = BettingChip.GetColor(this.Value);
		}

		private Task ClickedAsync() =>
			this.Clicked.InvokeAsync(this.Value);

		// @todo Based on algorithm
		private static string GetColor(ulong value) =>
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

		private static string Humanize(ulong value)
		{
			const float divide = 3f;

			var mag = (int)(float.Floor(float.Log10(value)) / divide);
			var divisor = float.Pow(10f, mag * divide);

			char? suffix = (mag) switch
			{
				3 => 'B',
				2 => 'M',
				1 => 'K',
				_ => null,
			};

			if (suffix is null)
			{
				return value.Str();
			}

			var rounded = float.Round(value / divisor, 1, System.MidpointRounding.ToZero);

			return string.Create(CultureInfo.InvariantCulture, $"{rounded:N0}{suffix}");
		}
	}
}
