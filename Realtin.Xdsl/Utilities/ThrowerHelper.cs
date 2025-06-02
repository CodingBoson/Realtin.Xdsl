using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Utilities;

internal static class ThrowerHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfArgumentNull<T>(string arg, [DisallowNull] T value) where T : class
	{
		if (value is null) {
			throw new ArgumentNullException(arg);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfArgumentNullOrEmpty(string arg, string value)
	{
		if (string.IsNullOrEmpty(value)) {
			throw new ArgumentNullException(arg);
		}
	}
}