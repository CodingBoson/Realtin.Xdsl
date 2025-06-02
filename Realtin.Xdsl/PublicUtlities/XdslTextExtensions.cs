namespace Realtin.Xdsl.Utilities;

/// <summary>
/// Provides functionality for working with XDSL safe strings.
/// </summary>
public static class XdslTextExtensions
{
	public static string Escape(this string s)
	{
		ThrowerHelper.ThrowIfArgumentNull(s, nameof(s));

		return $"@{{{s}}}";
	}

	public static string UnEscape(this string s)
	{
		ThrowerHelper.ThrowIfArgumentNull(s, nameof(s));

		if (s.StartsWithIgnoreWhiteSpace("@{", System.StringComparison.OrdinalIgnoreCase)
			&& s.EndsWithIgnoreWhiteSpace('}')) {
			return s[2..^1];
		}

		return s;
	}
}