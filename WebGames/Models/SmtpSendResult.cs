namespace WebGames.Models
{
	public enum SmtpSendResult
	{
		Success,
		ConnectionFailed,
		SslFailed,
		AuthenticationNotSupported,
		AuthenticationFailed,
		SendFailed,
	}
}
