namespace Realtin.Xdsl;

/// <summary>
/// Provides a class that processes conditional elements in a document.
/// </summary>
public sealed class XdslConditionalElementProcessor : IXdslDocumentProcessor<XdslConditions>
{
	/// <inheritdoc/>
	public void Process(XdslDocument document, XdslConditions options)
	{
		var root = document.Root;

		if (root == null) {
			return;
		}

		Process(root, options);
	}

	/// <inheritdoc/>
	public void Process(XdslElement element, XdslConditions options)
	{
		if (element.Children is null) {
			return;
		}

		var children = element.Children;
		for (int i = 0; i < children.Count; i++) {
			var child = children[i];

			if (child.TryGetAttribute("includeIf", out var includeIf)) {
				var condition = options.GetCondition(includeIf.Value);

				if (condition != null && !condition.IsChecked) {
					children.Remove(child);

					continue;
				}
			}

			Process(child, options);
		}
	}
}