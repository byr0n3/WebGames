using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Options;

namespace WebGames.Models.Options
{
	public sealed class SmtpOptions : IOptions<SmtpOptions>, ICredentials
	{
		public string? Host { get; set; }

		public int Port { get; set; } = 587;

		public string? Username { get; set; }

		public string? Password { get; set; }

		public bool UseSsl { get; set; }

		public bool ValidateSslCertificate { get; set; }

		public string DefaultFrom { get; set; } = nameof(WebGames);

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Host), nameof(this.Username), nameof(this.Password))]
			get => (this.Host is not null) && (this.Username is not null) && (this.Password is not null);
		}

		SmtpOptions IOptions<SmtpOptions>.Value =>
			this;

		public NetworkCredential GetCredential(Uri _, string __) =>
			new(this.Username, this.Password);
	}
}
