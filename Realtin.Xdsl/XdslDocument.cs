using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Realtin.Xdsl.NamingConventions;
using Realtin.Xdsl.Pooling;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL document. You can use this class to load, validate, edit, add,
/// and position XDSL in a document.
/// </summary>
[DebuggerDisplay("XdslDocument")]
public sealed class XdslDocument : XdslNode
{
	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override string? Text { get => string.Empty; set { } }

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override string Name { get => "Document"; set { } }

	/// <inheritdoc/>
	public override XdslNodeType NodeType => XdslNodeType.Document;

	/// <inheritdoc/>
	public override bool IsEmpty {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Children is null || Children.Count == 0;
	}

	// How to declare a version in Xdsl '<?xdsl version="1.0"?>'
	/// <summary>
	/// The version declared in the document.
	/// </summary>
	/// <returns>1.0 if the version is not declared.</returns>
	public XdslVersion Version {
		get {
			var xdslTag = GetChild<object?>(null, (state, x) => x.NodeType == XdslNodeType.Tag
			&& x.Name.Equals("XDSL", StringComparison.OrdinalIgnoreCase));

			if (xdslTag == null || !xdslTag.TryGetAttribute("Version", out var versionAttribute)) {
				return XdslVersion.OnePoint0;
			}

			return XdslVersion.Parse(versionAttribute.Value);
		}
		set {
			var xdslTag = GetChild<object?>(null, (state, x) => x.NodeType == XdslNodeType.Tag
			&& x.Name.Equals("XDSL", StringComparison.OrdinalIgnoreCase));

			if (xdslTag == null) {
				xdslTag = CreateTag("XDSL");

				MoveElement(0, xdslTag);
			}

			var versionAttribute = xdslTag.GetOrAddAttribute("Version");

			versionAttribute.Value = value.ToString();
		}
	}

	// How to declare a document type in Xdsl '<?xdsl docType="schema"?>'
	/// <summary>
	/// The document type declared in the document.
	/// </summary>
	/// <returns></returns>
	public XdslDocumentType DocType {
		get {
			var xdslTag = GetChild<object?>(null, (state, x) => x.NodeType == XdslNodeType.Tag
			&& x.Name.Equals("XDSL", StringComparison.OrdinalIgnoreCase));

			if (xdslTag == null || !xdslTag.TryGetAttribute("DocType", out var docTypeAttribute)) {
				return XdslDocumentType.Document;
			}

			return XdslDocumentType.Create(docTypeAttribute.Value);
		}
		set {
			var xdslTag = GetChild<object?>(null, (state, x) => x.NodeType == XdslNodeType.Tag
			&& x.Name.Equals("XDSL", StringComparison.OrdinalIgnoreCase));

			if (xdslTag == null) {
				xdslTag = CreateTag("XDSL");

				MoveElement(0, xdslTag);
			}

			var docTypeAttribute = xdslTag.GetOrAddAttribute("DocType");

			docTypeAttribute.Value = value.Type;
		}
	}

	/// <summary>
	/// The root element of the document.
	/// Tags and comments cannot be root elements.
	/// </summary>
	public XdslElement? Root {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (Children is null) {
				return null;
			}

			var length = Children.Count;
			for (int i = 0; i < length; i++) {
				var child = Children[i];

				if (child.NodeType == XdslNodeType.Element) {
					return child;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Gets or Sets a resource provider for this document.
	/// </summary>
	public IResourceProvider? ResourceProvider { get; set; }

	/// <summary>
	/// Gets an IEnumerable of imported documents. 
	/// </summary>
	public IEnumerable<string> ImportedDocuments {
		get {
			if (Children is null)
				yield break;

			foreach (var child in Children) {
				if (child.NodeType == XdslNodeType.Tag && child.IsSpecialName
					&& child.Name.EqualsOrdinalIgnoreCase("xdsl:import")) {
					if (child.TryGetAttribute("src", out var attribute)) {
						yield return attribute.Value;
					}
				}
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="XdslDocument"/> class.
	/// </summary>
	public XdslDocument() : base("Document", null)
	{
	}

	/// <summary>
	/// Moves an element from it's document to this document.
	/// </summary>
	/// <param name="newParent">The new parent</param>
	/// <param name="elementToMove">The element to move.</param>
	/// <exception cref="XdslException">The new parent element is not in this document.</exception>
	public void MoveElement(XdslElement newParent, XdslElement elementToMove)
	{
		if (newParent.Document != (object)this) {
			throw new XdslException($"The new parent element is not in this document.");
		}

		elementToMove.Parent.RemoveChild(elementToMove);

		newParent.AppendChild(elementToMove);
	}

	/// <summary>
	/// Moves an element from it's document to this document.
	/// </summary>
	/// <param name="elementToMove">The element to move.</param>
	public void MoveElement(XdslElement elementToMove)
	{
		elementToMove.Parent.RemoveChild(elementToMove);

		AppendChild(elementToMove);
	}

	/// <summary>
	/// Moves an element from it's current position to a new position specified
	/// by the <paramref name="newPosition"/> parameter.
	/// </summary>
	/// <param name="newPosition">The new position.</param>
	/// <param name="elementToMove">The element to reposition.</param>
	/// <exception cref="XdslException">The elementToMove is not in this document.</exception>
	public void MoveElement(int newPosition, XdslElement elementToMove)
	{
		if (elementToMove.Document != (object)this) {
			throw new XdslException("The elementToMove is not in this document.");
		}

		int oldPosition = elementToMove.Parent.Children!.IndexOf(elementToMove);

		elementToMove.Parent.Children!.RemoveAt(oldPosition);
		elementToMove.Parent.Children!.Insert(newPosition, elementToMove);
	}

	/// <summary>
	/// Moves an element in this document specified by the <paramref name="elementToReparent"/> parameter to the <paramref name="newParent"/>.
	/// </summary>
	/// <param name="newParent"></param>
	/// <param name="elementToReparent"></param>
	/// <exception cref="XdslException">The new parent element is not in this document. or The elementToReparent is not in this document.</exception>
	public void Reparent(XdslElement newParent, XdslElement elementToReparent)
	{
		if (newParent.Document == (object)this) {
			throw new XdslException("The new parent element is not in this document.");
		}

		if (elementToReparent.Document == (object)this) {
			throw new XdslException("The elementToReparent is not in this document.");
		}

		elementToReparent.Parent.RemoveChild(elementToReparent);
		newParent.AppendChild(elementToReparent);
	}

	/// <summary>
	/// Replaces the <paramref name="oldElement"/> with the <paramref name="newElement"/>.
	/// </summary>
	/// <param name="oldElement"></param>
	/// <param name="newElement"></param>
	/// <exception cref="XdslException">The old element is not in this document.</exception>
	public void Replace(XdslElement oldElement, XdslElement newElement)
	{
		if (oldElement.Document == (object)this) {
			throw new XdslException("The old element is not in this document.");
		}

		var position = oldElement.Parent.Children!.IndexOf(oldElement);

		oldElement.Parent.Children[position] = newElement;

		oldElement.Parent = null!;
	}

	/// <exception cref="XdslException"></exception>
	public void LoadXdsl(string xdsl, XdslDocumentOptions options)
	{
		Children?.Clear();

		var reader = new XdslTextReader(xdsl);

		reader.Load(this, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string WriteToString(bool writeIndented)
	{
		var builder = StringBuilderPool.Rent();
		//var builder = TempStringBuilder.Temp();

		var writer = XdslTextWriter.Create(builder);

		if (writeIndented) {
			WriteContentToIndented(writer);
		}
		else {
			WriteContentTo(writer);
		}

		var text = builder.ToString();

		StringBuilderPool.Return(builder);

		return text;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentToIndented(XdslTextWriter writer)
	{
		if (Children is not null) {
			var childCount = Children.Count;

			for (int i = 0; i < childCount; i++) {
				Children[i].WriteContentToIndented(writer);

				if (i < childCount - 1) {
					writer.WriteLine();
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentTo(XdslTextWriter writer)
	{
		if (Children is not null) {
			var childCount = Children.Count;

			for (int i = 0; i < childCount; i++) {
				Children[i].WriteContentTo(writer);
			}
		}
	}

	/// <summary>
	/// Check's if an XDSL feature is supported by this library.
	/// </summary>
	/// <param name="feature"></param>
	/// <param name="version"></param>
	/// <returns></returns>
	public bool SupportsFeature(string feature, string version)
	{
		if (string.Equals(feature, "XDSL", StringComparison.OrdinalIgnoreCase)) {
			// This library is compatible with Xdsl 1.0 and Xdsl 2.0.
			return version switch {
				"1.0" => true,
				"2.0" => true,
				_ => false,
			};
		}

		return false;
	}

	/// <summary>
	/// Changes the naming convention of all descendants using the specified <paramref name="convention"/>.
	/// </summary>
	/// <param name="convention"></param>
	public void TransformNamingConvention(INamingConvention convention)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(convention), convention);

		TransformNames(convention.Apply);
	}

	/// <summary>
	/// Transforms the names of all descendants using the specified <paramref name="transformation"/> function.
	/// </summary>
	/// <param name="transformation"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public void TransformNames(Func<string, string> transformation)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(transformation), transformation);

		if (Children != null) {
			for (int i = 0; i < Children.Count; i++) {
				var child = Children[i];

				TransformNames(child, transformation);
			}
		}
	}

	internal void TransformNames(XdslElement element, Func<string, string> transformation)
	{
		element.Name = transformation(element.Name);

		if (element.NodeType == XdslNodeType.Element && element.Children != null) {
			for (int i = 0; i < element.Children.Count; i++) {
				var child = element.Children[i];

				TransformNames(child, transformation);
			}
		}
	}

	/// <summary>
	/// Saves the XDSL document to the specified file. If the specified file exists, this
	/// method overwrites it.
	/// </summary>
	/// <param name="filePath">The location of the file where you want to save the document.</param>
	/// <param name="indented"></param>
	/// <exception cref="XdslException">
	/// The operation would not result in a well formed XDSL document (for example, no document element).
	/// </exception>
	public void Save(string filePath, bool indented = true)
	{
		ThrowerHelper.ThrowIfArgumentNullOrEmpty(nameof(filePath), filePath);

		if (Root is null) {
			throw new XdslException("The operation would not result in a well formed XDSL document (for example, no document element).");
		}

		File.WriteAllText(filePath, WriteToString(indented));
	}

	/// <summary>
	/// Saves the XDSL document to the specified file. If the specified file exists, this
	/// method overwrites it.
	/// </summary>
	/// <param name="filePath">The location of the file where you want to save the document.</param>
	/// <param name="indented"></param>
	/// <exception cref="XdslException">
	/// The operation would not result in a well formed XDSL document (for example, no document element).
	/// </exception>
	public async Task SaveAsync(string filePath, bool indented = true)
	{
		ThrowerHelper.ThrowIfArgumentNullOrEmpty(nameof(filePath), filePath);

		if (Root is null) {
			throw new XdslException("The operation would not result in a well formed XDSL document (for example, no document element).");
		}

		await File.WriteAllTextAsync(filePath, WriteToString(indented));
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <paramref name="xdsl"/> <see cref="string"/>.
	/// </summary>
	/// <param name="xdsl">A <see cref="string"/> representing an XDSL document.</param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslException"></exception>
	public static XdslDocument Create(string xdsl, XdslDocumentOptions? options = default)
	{
		options ??= XdslDocumentOptions.Default;

		if (string.IsNullOrEmpty(xdsl)) {
			return new XdslDocument();
		}

		var document = new XdslDocument();

		document.LoadXdsl(xdsl, options);

		return document;
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <paramref name="xdsl"/> <see cref="string"/>.
	/// </summary>
	/// <param name="xdsl">A <see cref="string"/> representing an XDSL document.</param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslException"></exception>
	[Obsolete("Use Create instead.")]
	public static Task<XdslDocument> CreateAsync(
		string xdsl,
		XdslDocumentOptions? options = default)
	{
		return Task<XdslDocument>.Factory.StartNew(() => Create(xdsl, options));
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <see cref="byte"/> array.
	/// </summary>
	/// <param name="bytes">A byte array representing an XDSL document.</param>
	/// <param name="encoding"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static XdslDocument Create(
		ReadOnlySpan<byte> bytes,
		Encoding encoding,
		XdslDocumentOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(encoding), encoding);

		var text = encoding.GetString(bytes);

		return Create(text, options);
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <see cref="byte"/> array.
	/// </summary>
	/// <param name="utf8Bytes">A byte array representing an XDSL document encoded in UTF-8.</param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static XdslDocument Create(
		ReadOnlySpan<byte> utf8Bytes,
		XdslDocumentOptions? options = default)
	{
		var text = Encoding.UTF8.GetString(utf8Bytes);

		return Create(text, options);
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <paramref name="fileName"/>.
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="IOException"></exception>
	/// <exception cref="XdslException"></exception>
	public static XdslDocument CreateFromFile(
		string fileName,
		XdslDocumentOptions? options = default)
	{
		var xdsl = File.ReadAllText(fileName);

		return Create(xdsl, options);
	}

	/// <summary>
	/// Creates an <see cref="XdslDocument"/> from the specified <paramref name="fileName"/>.
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="IOException"></exception>
	/// <exception cref="XdslException"></exception>
	public static async Task<XdslDocument> CreateFromFileAsync(
		string fileName,
		XdslDocumentOptions? options = default)
	{
		var xdsl = await File.ReadAllTextAsync(fileName);

		return Create(xdsl, options);
	}
}