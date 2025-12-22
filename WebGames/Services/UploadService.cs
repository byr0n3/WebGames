using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using SkiaSharp;
using WebGames.Models.Options;

namespace WebGames.Services
{
	public sealed class UploadService
	{
		private readonly UploadOptions options;

		public long MaxProfilePictureFileSize =>
			this.options.MaxProfilePictureFileSize;

		public UploadService(IOptions<UploadOptions> options) =>
			this.options = options.Value;

		// @todo Async support?
		public ProfilePictureUploadResult UploadProfilePicture(MemoryStream src, int userId)
		{
			var dstPath = this.options.GetProfilePicturePath(userId);

			using var dst = File.OpenWrite(dstPath);

			// Cheat to prevent allocating a new array.
			var buffer = src.GetBuffer();
			Array.Resize(ref buffer, (int)src.Position);

			using (var image = this.GetAndResizeImage(buffer))
			{
				if (image is null)
				{
					return ProfilePictureUploadResult.ErrorLoadImage;
				}

				using (var encoded = image.Encode(SKEncodedImageFormat.Webp, 75))
				{
					if (encoded is null)
					{
						return ProfilePictureUploadResult.ErrorConvertImage;
					}

					encoded.SaveTo(dst);
				}
			}

			return ProfilePictureUploadResult.Success;
		}

		[MustDisposeResource]
		private SKImage? GetAndResizeImage(byte[] buffer)
		{
			var srcImage = SKImage.FromEncodedData(buffer);

			if (srcImage is null)
			{
				return null;
			}

			// Image is already small enough, no need to resize.
			if ((srcImage.Width <= this.options.MaxProfilePictureDimensionSize) &&
				(srcImage.Height <= this.options.MaxProfilePictureDimensionSize))
			{
				return srcImage;
			}

			using (srcImage)
			using (var bitmap = SKBitmap.FromImage(srcImage))
			{
				if (bitmap is null)
				{
					return null;
				}

				// Get the smallest resizing factor.
				// This way, we can clamp the biggest dimension to the max. dimension size,
				// while the other dimension scales relatively.
				var resizeFactor = float.Min(
					(float)this.options.MaxProfilePictureDimensionSize / bitmap.Width,
					(float)this.options.MaxProfilePictureDimensionSize / bitmap.Height
				);

				var size = new SKSizeI((int)(bitmap.Width * resizeFactor), (int)(bitmap.Height * resizeFactor));

				using (var resized = bitmap.Resize(size, SKSamplingOptions.Default))
				{
					return resized != null ? SKImage.FromBitmap(resized) : null;
				}
			}
		}

		public enum ProfilePictureUploadResult
		{
			ErrorLoadImage,
			ErrorConvertImage,
			Success,
		}
	}
}
