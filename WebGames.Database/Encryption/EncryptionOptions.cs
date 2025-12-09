using System;
using Microsoft.Extensions.Options;

namespace WebGames.Database.Encryption
{
	public sealed class EncryptionOptions : IOptions<EncryptionOptions>
	{
		public const int KeyLength = 32;

		public byte[] Key
		{
			get;
			set
			{
				if (value.Length != EncryptionOptions.KeyLength)
				{
					throw new Exception($"Encryption key has to be exactly {KeyLength} bytes.");
				}

				field = value;
			}
		} = [];

		EncryptionOptions IOptions<EncryptionOptions>.Value =>
			this;
	}
}
