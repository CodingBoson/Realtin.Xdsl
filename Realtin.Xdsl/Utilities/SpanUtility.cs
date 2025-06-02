using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Utilities;

internal static class SpanUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith(this ReadOnlySpan<char> span, char c)
	{
		return span.Length > 0 && span[0] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWith(this ReadOnlySpan<char> span, char c)
	{
		int length = span.Length;

		return length > 0 && span[length - 1] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf(this ReadOnlySpan<char> span, int startIndex, char c)
	{
		return span[startIndex..].IndexOf(c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains(this ReadOnlySpan<char> span, char c)
	{
		return span.IndexOf(c) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithIgnoreWhiteSpace(this ReadOnlySpan<char> span, char c)
	{
		ReadOnlySpan<char> chars = span.TrimStart();

		int length = chars.Length;

		return length > 0 && chars[0] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithIgnoreWhiteSpace(this ReadOnlySpan<char> span, char c)
	{
		ReadOnlySpan<char> chars = span.TrimEnd();

		int length = chars.Length;

		return length > 0 && chars[length - 1] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithIgnoreWhiteSpace(this ReadOnlySpan<char> span,
		ReadOnlySpan<char> s, StringComparison comparison = StringComparison.CurrentCulture)
	{
		return span.TrimStart().StartsWith(s, comparison);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithIgnoreWhiteSpace(this ReadOnlySpan<char> span,
		ReadOnlySpan<char> s, StringComparison comparison = StringComparison.CurrentCulture)
	{
		return span.TrimEnd().EndsWith(s, comparison);
	}
}