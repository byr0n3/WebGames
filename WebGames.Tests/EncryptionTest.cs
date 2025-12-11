using Microsoft.Extensions.DependencyInjection;
using WebGames.Database.Encryption;
using Xunit;

namespace WebGames.Tests
{
	public sealed class EncryptionTest
	{
		private readonly DbEncryptor encryptor;

		public EncryptionTest(TestsServiceProvider provider)
		{
			this.encryptor = provider.GetRequiredService<DbEncryptor>();
		}

		[Fact]
		public void AssertCanEncryptAndDecrypt()
		{
			const string testValue = "Hello world! 🌎";

			var encrypted = this.encryptor.Encrypt(testValue);
			var decrypted = this.encryptor.Decrypt(encrypted);

			Assert.Equal(testValue, decrypted);
		}

		// @todo Test database comparison/encryption
	}
}
