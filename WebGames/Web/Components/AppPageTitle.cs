using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using WebGames.Models.Options;

namespace WebGames.Web.Components
{
	/// <inheritdoc cref="PageTitle"/>
	/// <remarks>
	/// This component also supports setting the page title for Interactive rendering mode,
	/// without doing weird business in the <c>App.razor</c>.
	/// </remarks>
	public sealed class AppPageTitle : ComponentBase
	{
		[Inject] public required IJSRuntime Js { get; init; }

		[Inject] public required IOptions<AppOptions> Options { get; init; }

		[Parameter] [EditorRequired] public required string Title { get; set; }

		private string FormattedTitle =>
			$"{this.Title} | {this.Options.Value.Name}";

		private bool UseJs =>
			this.AssignedRenderMode is not null;

		protected override Task OnAfterRenderAsync(bool firstRender)
		{
			if (!this.UseJs || !firstRender)
			{
				return Task.CompletedTask;
			}

			return this.Js.InvokeVoidAsync("setDocumentTitle", this.FormattedTitle).AsTask();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			if (this.UseJs)
			{
				return;
			}

			builder.OpenComponent<SectionContent>(0);
			builder.AddComponentParameter(1, nameof(SectionContent.SectionId), AppPageTitle.GetTitleSectionId(null));
			builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)this.BuildTitleRenderTree);
			builder.CloseComponent();
		}

		private void BuildTitleRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "title");
			builder.AddContent(1, this.FormattedTitle);
			builder.CloseElement();
		}

		[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "TitleSectionId")]
		private static extern ref object GetTitleSectionId(HeadOutlet? outlet);
	}
}
