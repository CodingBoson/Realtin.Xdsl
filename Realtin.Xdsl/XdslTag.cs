using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL tag.
/// </summary>
[DebuggerDisplay("Tag, {Name}")]
public sealed class XdslTag : XdslElement
{
	/// <inheritdoc/>
	public override string XNode
	{
		get {
			if (Attributes is null)
				return $"<?{Name}?>";

			return $"<?{Name} {Attributes}?>";
		}
	}

	/// <inheritdoc/>
	public override bool IsEmpty
		=> string.IsNullOrEmpty(Name)
		&& Attributes is null;

	/// <inheritdoc/>
	public override XdslNodeType NodeType => XdslNodeType.Tag;

	#region Not Supported

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override int TotalCount => 0;

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public override bool HasChildNodes => false;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

	/// <summary>
	/// <see cref="Children"/> is not supported by <see cref="XdslTag"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[Obsolete("Children is not supported by XdslTag", true)]
	public override XdslElementCollection? Children
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	{
		get => throw new NotSupportedException("XdslTag.Children is not supported by XdslTag.");
		protected internal set => throw new NotSupportedException("XdslTag.Children is not supported by XdslTag.");
	}

	#endregion Not Supported

	internal XdslTag(string name) : base(name)
	{
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentToIndented(XdslTextWriter writer)
	{
		WriteContentTo(writer);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void WriteContentTo(XdslTextWriter writer)
	{
		if (Attributes is null) {
			writer.Write("<?");
			writer.Write(Name);
			writer.Write("?>");

			return;
		}

		writer.Write("<?");
		writer.Write(Name);
		writer.Write(" ");

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

		writer.Write("?>");
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] XdslNode? other)
	{
		return other is not null
			&& other.Name == Name
			&& other.Text == Text
			&& other.NodeType == NodeType
			&& EqualityComparer<XdslAttributeCollection?>.Default.Equals(Attributes, other.Attributes);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(Name, Attributes);
	}
}