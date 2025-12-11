using System.Security.Cryptography;

namespace WebGames.Core.Utilities
{
	internal static class CodeGenerator
	{
		private const int codeLength = 4;
		private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static string Generate() =>
			RandomNumberGenerator.GetString(CodeGenerator.characters, CodeGenerator.codeLength);
	}
}
