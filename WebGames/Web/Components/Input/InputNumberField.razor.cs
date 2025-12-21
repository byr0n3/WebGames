using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components.Input
{
	public sealed partial class InputNumberField<TValue> : ComponentBase
		where TValue : INumber<TValue>
	{
		[Parameter] public required TValue? Value { get; set; }

		[Parameter] public required EventCallback<TValue?> ValueChanged { get; set; }

		[Parameter] public required Expression<Func<TValue?>> ValueExpression { get; set; }

		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter] public string? Help { get; set; }

		[Parameter] public bool Block { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
