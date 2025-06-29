using System;
using System.Collections.Generic;
using System.IO;

namespace Realtin.Xdsl.Experimental;

/// <summary>
/// This class is experimental and still a work in progress.
/// </summary>
internal sealed class LinkedElementProcessor : IXdslDocumentProcessor<XdslDocumentOptions>
{
	public List<(string Name, XdslDocument Document)> ImportedDocuments { get; } = [];

	public void Process(XdslDocument document, XdslDocumentOptions options)
	{
		if (document.Root is null)
		{
			return;
		}

		foreach (var import in document.ImportedDocuments)
		{
			var name = Path.GetFileNameWithoutExtension(import);

			if (document.ResourceProvider is null)
			{
				throw new XdslException("The provided document does not have a ResourceProvider.");
			}

			var stream = document.ResourceProvider.GetResourceStream(import);

			using var reader = new StreamReader(stream);

			var text = reader.ReadToEnd();

			var importedDoc = XdslDocument.Create(text);

			ImportedDocuments.Add((Name: name, Document: importedDoc));
		}

		Process(document.Root, options);
	}

	public void Process(XdslElement element, XdslDocumentOptions options)
	{
		if (element.Children is null)
		{
			return;
		}

		for (int i = 0; i < element.Children.Count; i++)
		{
			var child = element.Children[i];

			if (child.Name.Equals("linked:element", StringComparison.OrdinalIgnoreCase))
			{
				var source = child.GetAttribute("src")?.Value
					?? throw new XdslException("Missing src attribute.");

				var paths = source.Split('/');

				var from = paths[0];
				var elementName = paths[1];

				var (_, importedDocument) = ImportedDocuments.Find(x => x.Name == from);

				var elementToImport = importedDocument.GetChild(elementName)!;

				var doc = element.Document;

				doc.Replace(child, elementToImport);
			}
			else
			{
				Process(child, options);
			}
		}
	}
}