namespace Realtin.Xdsl.Xql;

/// <summary>
/// Represents a set of XQL Operations. 
/// </summary>
public enum XqlOperation : byte
{
	/// <summary>
	/// <see langword="SELECT"/>, Selects an element or elements from a node.
	/// </summary>
	Select,

	/// <summary>
	/// <see langword="DELETE"/>, Deletes an element or elements from a node.
	/// </summary>
	Delete
}