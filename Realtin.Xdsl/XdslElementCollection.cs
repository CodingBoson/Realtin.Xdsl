using System.Collections.Generic;

namespace Realtin.Xdsl;

/// <summary>
/// Represents a collection of XDSL elements.
/// </summary>
public sealed class XdslElementCollection : List<XdslElement>
{
	/// <summary>
	/// Initialize a new instance of the <see cref="XdslElementCollection"/> class.
	/// </summary>
	public XdslElementCollection()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslElementCollection"/> class.
	/// </summary>
	public XdslElementCollection(int capacity) : base(capacity)
	{
	}
}