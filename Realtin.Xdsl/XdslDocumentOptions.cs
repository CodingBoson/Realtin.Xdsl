using System;

namespace Realtin.Xdsl;

/// <summary>
/// Defines how the <see cref="XdslDocument"/> handles comments.
/// </summary>
public enum XdslCommentHandling : byte
{
	Ignore = 0,
	Parse = 1,
}

/// <summary>
/// Defines how the <see cref="XdslDocument"/> handles tags.
/// </summary>
public enum XdslTagHandling : byte
{
	Ignore = 0,
	Parse = 1,
}

/// <summary>
/// Provides the ability for the user to define custom behavior when parsing XDSL
/// to create an <see cref="XdslDocument"/>.
/// </summary>
public sealed class XdslDocumentOptions : IXdslDocumentOptions<XdslDocumentOptions>
{
	public static XdslDocumentOptions Default { get; } = new() {
		CommentHandling = XdslCommentHandling.Parse,
		TagHandling = XdslTagHandling.Parse,
	};

	/// <summary>
	/// Gets or sets a value that determines how the <see cref="XdslDocument"/> handles
	/// comments when reading through the XDSL data.
	/// </summary>
	public XdslCommentHandling CommentHandling { get; set; }

	/// <summary>
	/// Gets or sets a value that determines how the <see cref="XdslDocument"/> handles
	/// tags when reading through the XDSL data.
	/// </summary>
	public XdslTagHandling TagHandling { get; set; }

	public XdslDocumentOptions()
	{
	}

	public XdslDocumentOptions(XdslDocumentOptions options)
	{
		CommentHandling = options.CommentHandling;
		TagHandling = options.TagHandling;
	}

    public XdslDocumentOptions Clone() => new(this);

    public XdslDocumentOptions Clone(Action<XdslDocumentOptions> with)
	{
		var clone = Clone();

		with(clone);

		return clone;
	}
}