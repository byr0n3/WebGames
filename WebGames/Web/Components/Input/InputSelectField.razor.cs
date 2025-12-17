using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components.Input
{
	public sealed partial class InputSelectField<TValue> : ComponentBase
	{
		[Parameter] public required TValue Value { get; set; }

		[Parameter] public required EventCallback<TValue> ValueChanged { get; set; }

		[Parameter] public required Expression<Func<TValue>> ValueExpression { get; set; }

		[Parameter] [EditorRequired] public required IEnumerable<TValue> Options { get; set; }

		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter] public Func<TValue, string>? GetOptionLabel { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
