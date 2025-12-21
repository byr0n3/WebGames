using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WebGames.Models;
using WebGames.Models.Options;

namespace WebGames.Services
{
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
	public sealed class SmtpService : System.IDisposable
	{
		private readonly SmtpClient smtp;
		private readonly SmtpOptions options;
		private readonly ILogger<SmtpService> logger;

		public SmtpService(IOptions<SmtpOptions> options, ILogger<SmtpService> logger)
		{
			if (!options.Value.IsValid)
			{
				throw new System.ArgumentException("SMTP not configured", nameof(options));
			}

			this.smtp = new SmtpClient
			{
				CheckCertificateRevocation = options.Value.ValidateSslCertificate,
			};

			this.options = options.Value;
			this.logger = logger;
		}

		public async ValueTask<SmtpSendResult> SendAsync(SmtpMessageDescriptor descriptor, CancellationToken token = default)
		{
			Debug.Assert(this.options.IsValid);

			await this.smtp
					  .ConnectAsync(
						  this.options.Host,
						  this.options.Port,
						  this.options.UseSsl ? SecureSocketOptions.SslOnConnect : default,
						  token
					  )
					  .ConfigureAwait(false);

			if (!this.smtp.IsConnected)
			{
				this.logger.LogError("Failed to connect to SMTP server. Message: {Descriptor}", descriptor);

				return SmtpSendResult.ConnectionFailed;
			}

			if (this.options.UseSsl && !this.smtp.IsSecure)
			{
				this.logger.LogError("Failed to establish SSL while connecting to SMTP server. Message: {Descriptor}", descriptor);

				return SmtpSendResult.SslFailed;
			}

			if (!this.smtp.Capabilities.HasFlag(SmtpCapabilities.Authentication))
			{
				this.logger.LogError("SMTP server doesn't support authenticating. Message: {Descriptor}", descriptor);

				return SmtpSendResult.AuthenticationNotSupported;
			}

			await this.smtp.AuthenticateAsync(this.options, token).ConfigureAwait(false);

			if (!this.smtp.IsAuthenticated)
			{
				this.logger.LogError("Failed to authenticate with SMTP server. Message: {Descriptor}", descriptor);

				return SmtpSendResult.AuthenticationFailed;
			}

			using (var message = descriptor.ToMimeMessage(new MailboxAddress(this.options.DefaultFrom, this.options.Username)))
			{
				var response = await this.smtp.SendAsync(message, token).ConfigureAwait(false);

				if (!response.StartsWith("OK", System.StringComparison.Ordinal))
				{
					// @todo Parse/validate response

					this.logger.LogError("Failed to send SMTP message: {Response}. Message: {Descriptor}", response, descriptor);

					return SmtpSendResult.SendFailed;
				}
			}

			this.logger.LogDebug("Successfully sent: {Message}", descriptor);

			await this.smtp.DisconnectAsync(true, token).ConfigureAwait(false);

			Debug.Assert(!this.smtp.IsConnected);

			return SmtpSendResult.Success;
		}

		public void Dispose()
		{
			this.smtp.Dispose();
		}

		public readonly struct SmtpMessageDescriptor
		{
			public required MailboxAddress To { get; init; }

			public MailboxAddress[]? Cc { get; init; }

			public MailboxAddress[]? Bcc { get; init; }

			public MailboxAddress? From { get; init; }

			public required string Subject { get; init; }

			public string? TextBody { get; init; }

			public string? HtmlBody { get; init; }

			[MustDisposeResource]
			public MimeMessage ToMimeMessage(InternetAddress defaultFrom)
			{
				var message = new MimeMessage
				{
					To = { this.To },
					From = { this.From ?? defaultFrom },
					Subject = this.Subject,
					Body = new BodyBuilder
					{
						TextBody = this.TextBody,
						HtmlBody = this.HtmlBody,
					}.ToMessageBody(),
				};

				if (this.Cc is not null)
				{
					message.Cc.AddRange(this.Cc);
				}

				if (this.Bcc is not null)
				{
					message.Bcc.AddRange(this.Bcc);
				}

				return message;
			}

			public override string ToString() =>
				$"[{this.From?.ToString() ?? "Default"}] -> [{this.To}] {this.Subject}: {this.TextBody}";
		}
	}
}
