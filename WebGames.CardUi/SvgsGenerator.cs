using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace WebGames.CardUi
{
	[Generator]
	public sealed class SvgsGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var files = context.AdditionalTextsProvider
							   .Where(static (file) => Path.GetFileName(file.Path) is "cards.svg")
							   .Select(static (text, _) => text.Path)
							   .Collect();

			context.RegisterSourceOutput(files, SvgsGenerator.Generate);
		}

		private static void Generate(SourceProductionContext context, ImmutableArray<string> paths)
		{
			var builder = new StringBuilder();

			foreach (var path in paths)
			{
				var svg = XDocument.Load(path);

				if (svg.Root is not { } root)
				{
					continue;
				}

				foreach (var element in root.Elements(XName.Get("symbol", "http://www.w3.org/2000/svg")))
				{
					SvgsGenerator.AppendElement(builder, element);
				}
			}

			var source =
				// language=csharp
				$$"""
				  #nullable enable

				  using System;
				  using Microsoft.AspNetCore.Components;
				  using WebGames.Core.Cards;

				  namespace WebGames;

				  public static class CardUi
				  {
				  	public static RenderFragment Get(Card card)
				  	{
				  		return (card.Id) switch
				  		{
				  			{{builder}}
				  			_ => throw new ArgumentException($"Unknown card ID: {card.Id}", nameof(card)),
				  		};
				  	}
				  }
				  """;

			context.AddSource("CardUi.g.cs", source);
		}

		private static void AppendElement(StringBuilder builder, XElement element)
		{
			var id = element.Attribute("id")?.Value;

			if (string.IsNullOrEmpty(id))
			{
				return;
			}

			builder.AppendLine(
				// language=csharp
				$"{id} => (builder) => {{"
			);

			foreach (var child in element.Elements())
			{
				var name = child.Name.LocalName;

				builder.AppendLine($"builder.OpenElement(0, \"{name}\");");

				foreach (var attribute in child.Attributes())
				{
					var attrName = attribute.Name.LocalName;
					var attrValue = attribute.Value;

					builder.AppendLine($"builder.AddAttribute(0, \"{attrName}\", \"{attrValue}\");");
				}

				builder.AppendLine("builder.CloseElement();");
			}

			builder.Append(
				"},"
			);
		}
	}
}
