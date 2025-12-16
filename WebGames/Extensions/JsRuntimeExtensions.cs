using System;
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

			public ValueTask ConfettiResetAsync(CancellationToken token = default) =>
				js.InvokeVoidAsync("window.confetti.reset", token);
		}
	}

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
		public string[]? Colors { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public string[]? Shapes { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int Scalar { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int ZIndex { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public bool DisableForReducedMotion { get; init; }

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct ConfettiOrigin : System.IEquatable<ConfettiOrigin>
		{
			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
			public float X { get; init; }

			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
			public float Y { get; init; }

			public bool Equals(ConfettiOrigin other) =>
				this.X.Equals(other.X) && this.Y.Equals(other.Y);

			public override bool Equals(object? @object) =>
				(@object is ConfettiOrigin other) && this.Equals(other);

			public override int GetHashCode() =>
				HashCode.Combine(this.X, this.Y);

			public static bool operator ==(ConfettiOrigin left, ConfettiOrigin right) =>
				left.Equals(right);

			public static bool operator !=(ConfettiOrigin left, ConfettiOrigin right) =>
				!left.Equals(right);
		}
	}
}
