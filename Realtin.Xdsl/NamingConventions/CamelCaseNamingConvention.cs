using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.NamingConventions;

public sealed class CamelCaseNamingConvention : INamingConvention
{
	public static CamelCaseNamingConvention Instance { get; }
		= new CamelCaseNamingConvention();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Apply(string input)
	{
		if (input.Length == 0)
			return input;

		var firstChar = input[0];

		if (char.IsUpper(firstChar)) {
			return input.ToPascalOrCamelCaseFast(char.ToLowerInvariant);
		}

		return input;
	}
}