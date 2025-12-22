using System.IO;
using Elegance.Extensions;
using Microsoft.Extensions.Options;

namespace WebGames.Models.Options
{
	public sealed class UploadOptions : IOptions<UploadOptions>
	{
		public string RootPath { get; set; } = "./uploads";

		public long MaxProfilePictureFileSize = 1024 * 1024 * 25;

		public int MaxProfilePictureDimensionSize = 512;

		internal string GetDestinationPath(string path) =>
			Path.GetFullPath(Path.Combine(this.RootPath, path));

		internal string GetProfilePicturePath(int userId) =>
			Path.Combine(this.GetDestinationPath("profile-pictures"), $"{userId.Str()}.webp");

		internal string GetDefaultProfilePicturePath() =>
			Path.Combine(this.GetDestinationPath("profile-pictures"), "default.webp");

		UploadOptions IOptions<UploadOptions>.Value =>
			this;
	}
}
