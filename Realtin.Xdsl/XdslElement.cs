using Realtin.Xdsl.Pooling;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Realtin.Xdsl;

/// <summary>
/// Base class for all elements in XDSL.
/// </summary>
[DebuggerDisplay("Element, {Name}")]
public class XdslElement : XdslNode
{
	/// <summary>
	/// The parent of this element.
	/// <para>Note: May be null if this element is removed from it's parent
	/// node and not reparented.</para>
	/// </summary>
	public XdslNode Parent { get; internal set; } = null!;

	/// <summary>
	/// The parent document of this element.
	/// </summary>
	public XdslDocument Document
	{
		get {
			var parent = Parent;

			while (parent is not null) {
				if (parent is XdslDocument document) {
					return document;
				}

				parent = ((XdslElement)parent).Parent;
			}

			throw new InvalidOperationException("This element is not in a document. (???)");
		}
	}

	/// <inheritdoc/>
	public override XdslNodeType NodeType => XdslNodeType.Element;

	/// <summary>
	/// Initializes a new instance of the <see cref="XdslElement"/> class.
	/// </summary>
	/// <param name="name"></param>
	protected internal XdslElement(string name) : base(name, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="XdslElement"/> class.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="text"></param>
	protected internal XdslElement(string name, string? text) : base(name, text)
	{
	}

	/// <summary>
	/// Loads a resource from the document's resource provider.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="XdslException"></exception>
	public virtual byte[] LoadResource()
	{
		if (Document.ResourceProvider == null) {
			throw new XdslException($"ResourceProvider does not exist.");
		}

		var resourcePath = GetAttribute("src")?.Value ?? throw new XdslException($"Missing src attribute on '{Name}'.");

		return Document.ResourceProvider.GetResourceBytes(resourcePath);
	}

    /// <summary>
    /// Loads a resource from the document's resource provider.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="XdslException"></exception>
    public virtual async Task<byte[]> LoadResourceAsync()
    {
        if (Document.ResourceProvider == null) {
            throw new XdslException($"ResourceProvider does not exist.");
        }

        var resourcePath = GetAttribute("src")?.Value ?? throw new XdslException($"Missing src attribute on '{Name}'.");

        return await Document.ResourceProvider.GetResourceBytesAsync(resourcePath);
    }

    /// <summary>
    /// Loads a text resource from the document's resource provider.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="XdslException"></exception>
    public string LoadTextResource() => Encoding.UTF8.GetString(bytes: LoadResource());

    /// <summary>
    /// Loads a text resource from the document's resource provider.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="XdslException"></exception>
    public async Task<string> LoadTextResourceAsync()
    {
		var bytes = await LoadResourceAsync();

        return Encoding.UTF8.GetString(bytes);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentToIndented(XdslTextWriter writer)
	{
		if (Children is not null) {
			if (!string.IsNullOrEmpty(Text)) {
				WriteXNodeTo(writer, false);

				writer.EnterChild();
				writer.Write(Text);
				writer.ExitChild();
			}
			else {
				WriteXNodeTo(writer, false);
			}

			for (int i = 0; i < Children.Count; i++) {
				writer.EnterChild();

				Children[i].WriteContentToIndented(writer);

				writer.ExitChild();
			}

			writer.WriteLine();
			writer.Write("</");
			writer.Write(Name);
			writer.Write(">");
		}
		else {
			if (!string.IsNullOrEmpty(Text)) {
				WriteXNodeTo(writer, false);
				writer.Write(Text);
				writer.Write("</");
				writer.Write(Name);
				writer.Write('>');
			}
			else {
				WriteXNodeTo(writer, selfEnclosing: true);
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentTo(XdslTextWriter writer)
	{
		if (Children is not null) {
			WriteXNodeTo(writer, false);
			writer.Write(Text ?? string.Empty);

			for (int i = 0; i < Children.Count; i++) {
				Children[i].WriteContentTo(writer);
			}

			writer.Write("</");
			writer.Write(Name);
			writer.Write(">");
		}
		else {
			if (!string.IsNullOrEmpty(Text)) {
				WriteXNodeTo(writer, false);
				writer.Write(Text);
				writer.Write("</");
				writer.Write(Name);
				writer.Write('>');
			}
			else {
				WriteXNodeTo(writer, selfEnclosing: true);
			}
		}
	}

	// Optimized version.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WriteXNodeTo(XdslTextWriter writer, bool selfEnclosing)
	{
		if (Attributes is null) {
			writer.Write('<');
			writer.Write(Name);

			if (selfEnclosing) {
				writer.Write(" />");
			}
			else {
				writer.Write('>');
			}
		}
		else {
			writer.Write('<');
			writer.Write(Name);
			writer.Write(' ');

			bool first = true;
			for (int i = 0; i < Attributes.Count; i++) {
				if (first) {
					first = false;
				}
				else {
					writer.Write(' ');
				}

				var attribute = Attributes[i];

				// $"{Name}="{Value}""
				writer.Write(attribute.Name);
				writer.Write('=');
				writer.Write('"');
				writer.Write(attribute.Value);
				writer.Write('"');
			}

			if (selfEnclosing) {
				writer.Write(" />");
			}
			else {
				writer.Write('>');
			}
		}
	}

	// This method should only be used by the Serializer.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal string WriteToString(bool writeIndented)
	{
		//var builder = StringBuilderPool.Acquire();
		var builder = TempStringBuilder.Temp();

		var writer = XdslTextWriter.Create(builder);

		if (writeIndented) {
			WriteContentToIndented(writer);
		}
		else {
			WriteContentTo(writer);
		}

		var text = builder.ToString();

		//StringBuilderPool.Release(builder);

		return text;
	}
}