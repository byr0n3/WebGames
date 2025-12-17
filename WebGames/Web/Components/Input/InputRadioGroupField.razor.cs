using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components.Input
{
	public sealed partial class InputRadioGroupField<TValue, TOption> : ComponentBase
	{
		[Parameter] public required TValue Value { get; set; }

		[Parameter] public required EventCallback<TValue> ValueChanged { get; set; }

		[Parameter] public required Expression<Func<TValue>> ValueExpression { get; set; }

		[Parameter] [EditorRequired] public required IEnumerable<TOption> Options { get; set; }

		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter] public string? Help { get; set; }

		[Parameter] public Func<TOption, string>? GetOptionLabel { get; set; }

		[Parameter] public Func<TOption, string>? GetOptionValue { get; set; }

		[Parameter] public string? OptionClass { get; set; }

		[Parameter] public RenderFragment<TOption>? OptionContent { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
