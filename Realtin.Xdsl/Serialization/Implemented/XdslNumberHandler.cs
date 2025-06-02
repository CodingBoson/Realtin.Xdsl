using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

internal static class XdslNumberHandler
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseFloat(ReadOnlySpan<char> s, out float result)
	{
		if (float.TryParse(s, out result)) {
			return true;
		}

		if (s.Equals("Infinity", StringComparison.OrdinalIgnoreCase)) {
			result = float.PositiveInfinity;

			return true;
		}

		if (s.Equals("-Infinity", StringComparison.OrdinalIgnoreCase)) {
			result = float.NegativeInfinity;

			return true;
		}

		if (s.Equals("NaN", StringComparison.OrdinalIgnoreCase)) {
			result = float.NaN;

			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ParseFloat(ReadOnlySpan<char> s)
	{
		if (!TryParseFloat(s, out float result)) {
			throw new FormatException("s does not represent a number in a valid format.");
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseDouble(ReadOnlySpan<char> s, out double result)
	{
		if (double.TryParse(s, out result)) {
			return true;
		}

		if (s.Equals("Infinity", StringComparison.OrdinalIgnoreCase)) {
			result = double.PositiveInfinity;

			return true;
		}

		if (s.Equals("-Infinity", StringComparison.OrdinalIgnoreCase)) {
			result = double.NegativeInfinity;

			return true;
		}

		if (s.Equals("NaN", StringComparison.OrdinalIgnoreCase)) {
			result = double.NaN;

			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double ParseDouble(ReadOnlySpan<char> s)
	{
		if (!TryParseDouble(s, out double result)) {
			throw new FormatException("s does not represent a number in a valid format.");
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInt(ReadOnlySpan<char> s, out int result)
	{
		if (int.TryParse(s, out result)) {
			return true;
		}

		if (s.Equals("MaxValue", StringComparison.OrdinalIgnoreCase)) {
			result = int.MaxValue;

			return true;
		}

		if (s.Equals("-MinValue", StringComparison.OrdinalIgnoreCase)) {
			result = int.MinValue;

			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ParseInt(ReadOnlySpan<char> s)
	{
		if (!TryParseInt(s, out int result)) {
			throw new FormatException("s does not represent a number in a valid format.");
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseLong(ReadOnlySpan<char> s, out long result)
	{
		if (long.TryParse(s, out result)) {
			return true;
		}

		if (s.Equals("MaxValue", StringComparison.OrdinalIgnoreCase)) {
			result = long.MaxValue;

			return true;
		}

		if (s.Equals("-MinValue", StringComparison.OrdinalIgnoreCase)) {
			result = long.MinValue;

			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long ParseLong(ReadOnlySpan<char> s)
	{
		if (!TryParseLong(s, out long result)) {
			throw new FormatException("s does not represent a number in a valid format.");
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseDecimal(ReadOnlySpan<char> s, out decimal result)
	{
		if (decimal.TryParse(s, out result)) {
			return true;
		}

		if (s.Equals("MaxValue", StringComparison.OrdinalIgnoreCase)) {
			result = decimal.MaxValue;

			return true;
		}

		if (s.Equals("-MinValue", StringComparison.OrdinalIgnoreCase)) {
			result = decimal.MinValue;

			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal ParseDecimal(ReadOnlySpan<char> s)
	{
		if (!TryParseDecimal(s, out decimal result)) {
			throw new FormatException("s does not represent a number in a valid format.");
		}

		return result;
	}
}