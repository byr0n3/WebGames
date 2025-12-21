using System.IO;
using Elegance.Extensions;
using Microsoft.Extensions.Options;

namespace WebGames.Models.Options
{
	public sealed class UploadOptions : IOptions<UploadOptions>
	{
		public string RootPath { get; set; } = "./uploads";

		public long MaxProfilePictureFileSize = 1024 * 1024 * 25;

		public int MaxProfilePictureDimensions = 512;

		internal string GetDestinationPath(string path) =>
			Path.GetFullPath(Path.Combine(this.RootPath, path));

		internal string GetProfilePicturePath(int userId) =>
			Path.Combine(this.GetDestinationPath("profile-pictures"), $"{userId.Str()}.webp");

		UploadOptions IOptions<UploadOptions>.Value =>
			this;
	}
}
