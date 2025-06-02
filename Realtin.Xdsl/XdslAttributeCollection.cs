using System.Collections.Generic;

namespace Realtin.Xdsl;

/// <summary>
/// Represents a collection of XDSL attributes.
/// </summary>
public sealed class XdslAttributeCollection : List<XdslAttribute>
{
	/// <summary>
	/// Initialize a new instance of the <see cref="XdslAttributeCollection"/> class.
	/// </summary>
	public XdslAttributeCollection()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslAttributeCollection"/> class.
	/// </summary>
	public XdslAttributeCollection(int capacity) : base(capacity)
	{
	}

	/// <inheritdoc/>
	public override string ToString() => string.Join(" ", this);
}