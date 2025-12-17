using System;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components
{
	public sealed partial class Collapsable : ComponentBase
	{
		[Parameter] public string Id { get; set; } = Guid.NewGuid().ToString();

		[Parameter] [EditorRequired] public required RenderFragment TriggerContent { get; set; }

		[Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }

		[Parameter] public string TriggerClass { get; set; } = string.Empty;

		[Parameter] public string ContentClass { get; set; } = string.Empty;

		[Parameter] public bool DefaultVisible { get; set; }
	}
}
