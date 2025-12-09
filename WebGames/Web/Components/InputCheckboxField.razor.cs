using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebGames.Web.Components
{
	public sealed partial class InputCheckboxField : ComponentBase
	{
		[Parameter] public required bool Value { get; set; }

		[Parameter] public required EventCallback<bool> ValueChanged { get; set; }

		[Parameter] public required Expression<Func<bool>> ValueExpression { get; set; }

		[Parameter] [EditorRequired] public required string Id { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
	}
}
