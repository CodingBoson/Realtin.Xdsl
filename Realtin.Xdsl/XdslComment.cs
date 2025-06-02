using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL comment.
/// </summary>
[DebuggerDisplay("Comment, {Text}")]
public sealed class XdslComment : XdslElement
{
	/// <inheritdoc/>
	public override string XNode => $"<!--{Text}-->";

	/// <inheritdoc/>
	public override bool IsEmpty => string.IsNullOrEmpty(Text);

	/// <inheritdoc/>
	public override XdslNodeType NodeType => XdslNodeType.Comment;

	#region Not Supported

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override int TotalCount => 0;

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override bool HasAttributes => false;

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override bool HasChildNodes => false;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

	/// <summary>
	/// <see cref="Attributes"/> is not supported by <see cref="XdslComment"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[Obsolete("Children is not supported by XdslComment.")]
	public override XdslAttributeCollection? Attributes
	{
		get => throw new NotSupportedException("XdslComment.Attributes is not supported by XdslComment.");
		protected internal set => throw new NotSupportedException("XdslComment.Attributes is not supported by XdslComment.");
	}

	/// <summary>
	/// <see cref="Children"/> is not supported by <see cref="XdslComment"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[Obsolete("Children is not supported by XdslComment.")]
	public override XdslElementCollection? Children
	{
		get => throw new NotSupportedException("XdslComment.Children is not supported by XdslComment.");
		protected internal set => throw new NotSupportedException("XdslComment.Children is not supported by XdslComment.");
	}

#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

	#endregion Not Supported

	internal XdslComment(string text) : base("Comment", text)
	{
	}

	[DoesNotReturn]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override byte[] LoadResource() 
		=> throw new NotSupportedException("XdslComment does not support loading resources.");

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentToIndented(XdslTextWriter writer) => WriteContentTo(writer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentTo(XdslTextWriter writer)
	{
		writer.Write("<!--");
		writer.Write(Text);
		writer.Write("-->");
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] XdslNode? other) => other is not null
			&& other.Text == Text
			&& other.NodeType == NodeType;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Name, Text);
}