using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components
{
	public sealed partial class Card : ComponentBase
	{
		[Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }

		[Parameter] [EditorRequired] public required RenderFragment HeaderContent { get; set; }

		[Parameter] public RenderFragment? FooterContent { get; set; }

		[Parameter] public string CardClass { get; set; } = string.Empty;

		[Parameter] public string HeaderClass { get; set; } = string.Empty;

		[Parameter] public string ContentClass { get; set; } = string.Empty;

		[Parameter] public string FooterClass { get; set; } = string.Empty;
	}
}
