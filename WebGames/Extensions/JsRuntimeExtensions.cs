using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebGames.Extensions
{
	internal static class JsRuntimeExtensions
	{
		extension(IJSRuntime js)
		{
			public ValueTask ConfettiAsync(in ConfettiConfig config, CancellationToken token = default) =>
				js.InvokeVoidAsync("window.confetti", token, config);
		}
	}

	// @todo Make `JsonIgnoreCondition.WhenWritingDefault` default
	public readonly struct ConfettiConfig
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int ParticleCount { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Angle { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Spread { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int StartVelocity { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public float Decay { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public float Gravity { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Drift { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public bool Flat { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Ticks { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public ConfettiOrigin Origin { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public string[] Colors { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public string[] Shapes { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Scalar { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int ZIndex { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public bool DisableForReducedMotion { get; init; }

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct ConfettiOrigin
		{
			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
			public float X { get; init; }

			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
			public float Y { get; init; }
		}
	}
}
