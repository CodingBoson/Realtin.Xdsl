using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.NamingConventions;

public sealed class PascalCaseNamingConvention : INamingConvention
{
	public static PascalCaseNamingConvention Instance { get; }
		= new PascalCaseNamingConvention();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Apply(string input)
	{
		if (input.Length == 0)
			return input;

		var firstChar = input[0];
		if (char.IsLower(firstChar)) {
			return input.ToPascalOrCamelCaseFast(char.ToUpperInvariant);
		}

		return input;
	}
}