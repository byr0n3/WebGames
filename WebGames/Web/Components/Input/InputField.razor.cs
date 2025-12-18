using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components.Input
{
	public sealed partial class InputField<TValue> : ComponentBase
	{
		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }

		[Parameter] [EditorRequired] public required Expression<Func<TValue>> ValueExpression { get; set; }

		[Parameter] public string? Help { get; set; }

		[Parameter] public bool Block { get; set; }

		[Parameter] public bool LabelBlock { get; set; } = true;
	}
}
