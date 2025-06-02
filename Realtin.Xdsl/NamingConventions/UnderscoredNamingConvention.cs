using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.NamingConventions;

public sealed class UnderscoredNamingConvention : INamingConvention
{
	public static UnderscoredNamingConvention Instance { get; }
		= new UnderscoredNamingConvention();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Apply(string input)
	{
		// Use string.Create which is way faster than string.Concat.
		// Also string.Concat boxes it's arguments.
		return string.Create(input.Length + 1, input, (span, state) => {
			span[0] = '_';

			state.AsSpan().CopyTo(span[1..]);

			if (span.Length >= 2) {
				span[1] = char.ToLower(span[1]);
			}
		});

		// Slow version
		//var firstChar = input[0];

		//if (firstChar != '_') {
		//	return '_' + CamelCaseNamingConvention.Instance.Apply(input);
		//}

		//return input;
	}
}