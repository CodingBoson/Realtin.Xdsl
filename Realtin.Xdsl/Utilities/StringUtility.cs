using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Realtin.Xdsl.Utilities;

internal static class StringUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Equals(string? a, string? b)
	{
		return string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b) || a == b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithIgnoreWhiteSpace(this string s, char c)
	{
		ReadOnlySpan<char> chars = ((ReadOnlySpan<char>)s).TrimStart();

		int length = chars.Length;

		return length > 0 && chars[0] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithIgnoreWhiteSpace(this string s, char c)
	{
		ReadOnlySpan<char> chars = ((ReadOnlySpan<char>)s).TrimEnd();

		int length = chars.Length;

		return length > 0 && chars[length - 1] == c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithIgnoreWhiteSpace(this string s, string str, StringComparison comparison)
	{
		return ((ReadOnlySpan<char>)s).StartsWithIgnoreWhiteSpace(str, comparison);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithIgnoreWhiteSpace(this string s, string str)
	{
		return ((ReadOnlySpan<char>)s).EndsWithIgnoreWhiteSpace(str);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EqualsOrdinalIgnoreCase(this string s, string other)
	{
		if (ReferenceEquals(s, other)) {
			return true;
		}

		return s is not null && other is not null && s.Equals(other, StringComparison.OrdinalIgnoreCase);
	}

	[Obsolete("Use ToPascalOrCamelCaseFast instead.")]
	public static string ToPascalOrCamelCase(this string str, Func<char, char> firstLetterTransform)
	{
		string text = Regex.Replace(str, "([_\\-])(?<char>[a-z])", (Match match) => match.Groups["char"].Value.ToUpperInvariant(), RegexOptions.IgnoreCase);
		return firstLetterTransform(text[0]) + text[1..];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToPascalOrCamelCaseFast(this string str, Func<char, char> firstLetterTransform)
	{
		return string.Create(str.Length, (str, firstLetterTransform), (span, state) => {
			state.str.AsSpan().CopyTo(span);

			span[0] = state.firstLetterTransform(span[0]);
		});
	}
}