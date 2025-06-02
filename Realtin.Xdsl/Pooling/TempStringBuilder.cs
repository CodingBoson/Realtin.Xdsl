using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Realtin.Xdsl.Pooling;

internal static class TempStringBuilder
{
	[ThreadStatic]
	private static StringBuilder? _temp;

	// Gets a thread safe temporary System.Text.StringBuilder.
	// Note: Make sure no other method calls Temp while the
	// temporary System.Text.StringBuilder. is in use.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder Temp()
	{
		// Create a new string builder for the current thread if needed.
		_temp ??= new StringBuilder(256);

		// Clear the string builder
		_temp.Clear();

		return _temp;
	}
}