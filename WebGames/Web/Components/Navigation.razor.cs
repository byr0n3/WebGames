using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace WebGames.Web.Components
{
	public sealed partial class Navigation : ComponentBase
	{
		[Inject] public required IStringLocalizer<NavigationLocalization> Localizer { get; init; }
	}
}
