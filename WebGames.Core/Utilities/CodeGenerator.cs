using System.Security.Cryptography;

namespace WebGames.Core.Utilities
{
	internal static class CodeGenerator
	{
		public const int CodeLength = 4;
		private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static string Generate(int length = CodeGenerator.CodeLength) =>
			RandomNumberGenerator.GetString(CodeGenerator.characters, length);
	}
}
