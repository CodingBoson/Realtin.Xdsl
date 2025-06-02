using System.Collections.Generic;

namespace Realtin.Xdsl.Xql;

/// <summary>
/// Represents XQL variables.
/// </summary>
public sealed class XqlVariables : Dictionary<string, string>
{
	public static XqlVariables Empty { get; } = [];
}