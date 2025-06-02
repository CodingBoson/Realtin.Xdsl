namespace Realtin.Xdsl.Xql;

/// <summary>
/// Specifies what property to access from an <see cref="XdslNode"/>.
/// </summary>
public enum XqlProperty : byte
{
	/// <summary>
	/// Specifies a node's name.
	/// </summary>
	Name,

	/// <summary>
	/// Specifies a node's type.
	/// </summary>
	Type,

	/// <summary>
	/// Specifies a node's text.
	/// </summary>
	Text,

	/// <summary>
	/// Specifies a node's child.
	/// </summary>
	Child,

	/// <summary>
	/// Specifies a node's attribute.
	/// </summary>
	Attribute,
}