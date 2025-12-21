using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SkiaSharp;
using WebGames.Extensions;
using WebGames.Models;
using WebGames.Models.Options;
using WebGames.Services;

namespace WebGames.Web.Components
{
	public sealed partial class ProfilePictureUpload : ComponentBase
	{
		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required IOptions<UploadOptions> Options { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<ProfilePictureUploadLocalization> Localizer { get; init; }

		private string? error;

		private async Task OnUploadAsync(InputFileChangeEventArgs args)
		{
			this.error = this.ValidateFile(args);

			var userId = this.Authentication.User?.GetClaimValue<int>(ClaimType.Id) ?? default;

			var src = await CopyFileToMemoryAsync(args.File, this.Options.Value.MaxProfilePictureFileSize);
			var dst = File.OpenWrite(this.Options.Value.GetProfilePicturePath(userId));

			await using (src)
			await using (dst)
			{
				this.error = this.WriteWebP(src, dst);
			}

			if (this.error is null)
			{
				this.Navigation.Refresh(true);
			}

			return;

			// @todo Let IBrowserFile use sync-IO.
			static async ValueTask<MemoryStream> CopyFileToMemoryAsync(IBrowserFile file, long maxSize)
			{
				var src = new MemoryStream((int)file.Size);

				await using (var fileStream = file.OpenReadStream(maxSize))
				{
					await fileStream.CopyToAsync(src);
				}

				return src;
			}
		}

		// @todo Move to service
		private string? WriteWebP(MemoryStream src, FileStream dst)
		{
			// Cheat to prevent allocating a new array.
			var buffer = src.GetBuffer();
			Array.Resize(ref buffer, (int)src.Position);

			using var srcImage = GetImage(buffer, this.Options.Value.MaxProfilePictureDimensions);

			if (srcImage is null)
			{
				return this.Localizer["ErrorLoadImage"];
			}

			using var encoded = srcImage.Encode(SKEncodedImageFormat.Webp, 75);

			if (encoded is null)
			{
				return this.Localizer["ErrorConvertImage"];
			}

			encoded.SaveTo(dst);

			return null;

			// @todo Refactor
			static SKImage? GetImage(byte[] buffer, int maxDimensions)
			{
				var srcImage = SKImage.FromEncodedData(buffer);

				if (srcImage is null)
				{
					return null;
				}

				if ((srcImage.Width < maxDimensions) && (srcImage.Height < maxDimensions))
				{
					return srcImage;
				}

				using var bitmap = SKBitmap.FromImage(srcImage);

				if (bitmap is null)
				{
					return null;
				}

				var (targetWidth, targetHeight) = GetResizeDimensions(bitmap.Width, bitmap.Height, maxDimensions);

				srcImage.Dispose();

				using var resized = bitmap.Resize(new SKSizeI(targetWidth, targetHeight), SKSamplingOptions.Default);

				return resized != null ? SKImage.FromBitmap(resized) : null;
			}

			static (int Width, int Height) GetResizeDimensions(int width, int height, int maxDimensions)
			{
				var resizeFactorWidth = (float)maxDimensions / width;
				var resizeFactorHeight = (float)maxDimensions / height;

				var resizeFactor = float.Min(resizeFactorWidth, resizeFactorHeight);

				return ((int)(width * resizeFactor), (int)(height * resizeFactor));
			}
		}

		private string? ValidateFile(InputFileChangeEventArgs args)
		{
			if (args.FileCount != 1)
			{
				return this.Localizer["ErrorFileCount"];
			}

			if (!args.File.ContentType.StartsWith("image/", StringComparison.Ordinal))
			{
				return this.Localizer["ErrorContentType"];
			}

			if (args.File.Size > this.Options.Value.MaxProfilePictureFileSize)
			{
				return this.Localizer["ErrorFileSize"];
			}

			return null;
		}
	}
}
