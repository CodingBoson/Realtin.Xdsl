using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Realtin.Xdsl.Syntax;

internal static class SyntaxHelper
{
	/* <NameSchema>
	 *		First Character:
	 *		_, Letters
	 *
	 *		Fallowing Characters:
	 *		_, -, Letters, Numbers
	 * </NameSchema>
	 */

	private const char underscore = '_';

	private const char hyphen = '-';

	public static void AssertValidName(string name)
	{
		if (string.IsNullOrEmpty(name)) {
			throw new ArgumentNullException("name");
		}

		ReadOnlySpan<char> chars = name;

		var firstChar = chars[0];
		if (!char.IsLetter(firstChar) && firstChar != underscore) {
			throw new XdslException($"Name '{name}' is not a valid XDSL name.");
		}

		for (int i = 0; i < chars.Length; i++) {
			char c = chars[i];

			if (!char.IsLetterOrDigit(c) && c != underscore && c != hyphen) {
				switch (c) {
					case underscore:
						continue;
					case hyphen:
						continue;
					case '.':
						continue;
					case ':':
						continue;
					default:
						throw new XdslException($"Name '{name}' is not a valid XDSL name.");
				}
			}
		}
	}

	[Obsolete("Use LegalNameFast or LegalNameMemoryOptimized instead. This method is also out of date.")]
	public static string LegalXNameSlow(string name)
	{
		if (name.Length == 0) {
			return name; // Throw
		}

		var builder = new StringBuilder(name.Length);

		var firstChar = name[0];

		if (firstChar != underscore && !char.IsLetter(firstChar)) {
			builder.Append(underscore);
		}
		else {
			builder.Append(firstChar);
		}

		for (int i = 1; i < name.Length; i++) {
			char c = name[i];

			if (!char.IsLetterOrDigit(c) && c != underscore && c != hyphen) {
				builder.Append(underscore);

				continue;
			}

			builder.Append(c);
		}

		return builder.ToString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string LegalNameFast(string _name)
	{
		if (_name.Length == 0) {
			return _name; // Throw
		}

		return string.Create(_name.Length, _name, (span, state) => {
			state.AsSpan().CopyTo(span);

			var firstChar = span[0];

			if (firstChar != underscore && !char.IsLetter(firstChar)) {
				span[0] = underscore;
			}

			int length = span.Length;
			for (int i = 1; i < length; i++) {
				char c = span[i];

				if (!char.IsLetterOrDigit(c) && c != underscore && c != hyphen && c != '.') {
					span[i] = underscore;

					continue;
				}
			}
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string LegalNameMemoryOptimized(string name)
	{
		if (name.Length == 0) {
			return name; // Throw
		}

		bool isIllegal = false;

		var firstChar = name[0];

		if (firstChar != underscore && !char.IsLetter(firstChar)) {
			isIllegal = true;
		}
		else {
			int length = name.Length;
			for (int i = 1; i < length; i++) {
				char c = name[i];

				if (!char.IsLetterOrDigit(c) && c != underscore && c != hyphen && c != '.') {
					isIllegal = true;

					break;
				}
			}
		}

		if (!isIllegal) {
			return name;
		}

		return LegalNameFast(name);
	}
}