namespace Realtin.Xdsl.Xql;

/// <summary>
/// Represents an XQL query.
/// </summary>
public enum XqlQueryType : byte
{
	/// <summary>
	/// <see langword="WHERE"/>, Specifies that the XQL operation should
	/// be performed on multiple elements that satisfy a condition.
	/// </summary>
	Where,

	/// <summary>
	/// <see langword="FIRST"/>, Specifies that the XQL operation should
	/// be performed on the first element that satisfies a condition.
	/// </summary>
	First
}