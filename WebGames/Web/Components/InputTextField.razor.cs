using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components
{
	public sealed partial class InputTextField : ComponentBase
	{
		[Parameter] public required string? Value { get; set; }

		[Parameter] public required EventCallback<string?> ValueChanged { get; set; }

		[Parameter] public required Expression<Func<string?>> ValueExpression { get; set; }

		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
