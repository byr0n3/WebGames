using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace WebGames.Database.Encryption
{
	public sealed class DbEncryptor
	{
		private readonly EncryptionOptions options;

		public DbEncryptor(IOptions<EncryptionOptions> options)
		{
			this.options = options.Value;
		}

		public byte[] Encrypt(string value)
		{
			using (var aes = this.Create())
			using (var buffer = new MemoryStream())
			{
				using (var crypto = new CryptoStream(buffer, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
				using (var writer = new StreamWriter(crypto))
				{
					writer.Write(value);
				}

				return buffer.ToArray();
			}
		}

		public string Decrypt(byte[] value)
		{
			using (var aes = this.Create())
			{
				using (var buffer = new MemoryStream(value))
				using (var crypto = new CryptoStream(buffer, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
				using (var reader = new StreamReader(crypto))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private Aes Create()
		{
			var aes = Aes.Create();
			{
				aes.Mode = CipherMode.CBC;
				aes.KeySize = 256;
				aes.BlockSize = aes.KeySize / 2;
				aes.Key = this.options.Key;
				// @todo Support different IV per encrypted data/model
				// Currently fixed to compare encrypted values in DB without the encrypted values differing.
				aes.IV = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16];
				aes.Padding = PaddingMode.PKCS7;
			}
			return aes;
		}
	}
}
