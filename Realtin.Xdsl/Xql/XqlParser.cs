using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Xql;

internal static class XqlParser
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlOperator ParseOperator(ReadOnlySpan<char> chars)
	{
		if (chars.Equals("==", StringComparison.OrdinalIgnoreCase)) {
			return XqlOperator.Equals;
		}
		else if (chars.Equals("!=", StringComparison.OrdinalIgnoreCase)) {
			return XqlOperator.NotEquals;
		}

		throw new XqlException($"Operator {chars.ToString()} is not a recognized Operator.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlQueryType ParseQuery(ReadOnlySpan<char> chars)
	{
		if (chars.Equals("WHERE", StringComparison.OrdinalIgnoreCase)) {
			return XqlQueryType.Where;
		}
		else if (chars.Equals("FIRST", StringComparison.OrdinalIgnoreCase)) {
			return XqlQueryType.First;
		}

		throw new XqlException($"Query {chars.ToString()} is not a recognized Query.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlOperation ParseMethod(ReadOnlySpan<char> chars)
	{
		if (chars.Equals("SELECT", StringComparison.OrdinalIgnoreCase)) {
			return XqlOperation.Select;
		}
		else if (chars.Equals("DELETE", StringComparison.OrdinalIgnoreCase)) {
			return XqlOperation.Delete;
		}

		throw new XqlException($"Method {chars.ToString()} is not a recognized Method.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslNodeType ParseNodeType(ReadOnlySpan<char> chars)
	{
		if (chars.Equals(nameof(XdslNodeType.Document), StringComparison.OrdinalIgnoreCase)) {
			return XdslNodeType.Document;
		}
		else if (chars.Equals(nameof(XdslNodeType.Element), StringComparison.OrdinalIgnoreCase)) {
			return XdslNodeType.Element;
		}
		else if (chars.Equals(nameof(XdslNodeType.Tag), StringComparison.OrdinalIgnoreCase)) {
			return XdslNodeType.Tag;
		}
		else if (chars.Equals(nameof(XdslNodeType.Comment), StringComparison.OrdinalIgnoreCase)) {
			return XdslNodeType.Comment;
		}

		throw new XqlException($"NodeType {chars.ToString()} is not a recognized NodeType.");
	}
}