using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using WebGames.Extensions;
using WebGames.Models;
using WebGames.Services;

namespace WebGames.Web.Components
{
	public sealed partial class ProfilePictureUpload : ComponentBase
	{
		[Inject] public required UploadService Upload { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required IStringLocalizer<ProfilePictureUploadLocalization> Localizer { get; init; }

		private string? error;

		private async Task OnUploadAsync(InputFileChangeEventArgs args)
		{
			Debug.Assert(this.Authentication.User is not null);

			this.error = this.ValidateFile(args);

			if (this.error is not null)
			{
				return;
			}

			var userId = this.Authentication.User.GetClaimValue<int>(ClaimType.Id);

			var src = await CopyFileToMemoryAsync(args.File, this.Upload.MaxProfilePictureFileSize);

			await using (src)
			{
				var result = this.Upload.UploadProfilePicture(src, userId);

				if (result == UploadService.ProfilePictureUploadResult.Success)
				{
					this.Navigation.Refresh(true);
				}
				else
				{
					this.error = this.Localizer[result.ToString()];
				}
			}

			return;

			// @todo Let IBrowserFile use sync-IO.
			[MustDisposeResource]
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

			if (args.File.Size > this.Upload.MaxProfilePictureFileSize)
			{
				return this.Localizer["ErrorFileSize"];
			}

			return null;
		}
	}
}
