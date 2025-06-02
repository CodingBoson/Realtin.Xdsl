using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.NamingConventions;

public sealed class EmptyNamingConvention : INamingConvention
{
	public static readonly EmptyNamingConvention Empty = new();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Apply(string input) => input;
}