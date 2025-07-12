using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL version.
/// </summary>
/// <param name="major"></param>
/// <param name="minor"></param>
public readonly struct XdslVersion(int major, int minor) : IEquatable<XdslVersion>
{
	public static XdslVersion OnePoint0 { get; } = new(1, 0);

	/// <summary>
	/// Gets the value of the major component of the version number
	/// for the current <see cref="XdslVersion"/> object.
	/// </summary>
	public int Major { get; } = major;

	/// <summary>
	/// Gets the value of the minor component of the version number
	/// for the current <see cref="XdslVersion"/> object.
	/// </summary>
	public int Minor { get; } = minor;

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is XdslVersion other && Equals(other);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] XdslVersion other) => Major == other.Major && Minor == other.Minor;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Major, Minor);

	/// <inheritdoc/>
	public override string ToString() => $"{Major}.{Minor}";

	/// <summary>
	/// Parsees the specified <paramref name="version"/> <see cref="string"/> as an <see cref="XdslVersion"/>.
	/// </summary>
	/// <param name="version"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslVersion Parse(string version)
	{
		if (version == null) {
			throw new ArgumentNullException("version");
		}

		return Parse((ReadOnlySpan<char>)version);
	}

	/// <summary>
	/// Parsees the specified <paramref name="version"/> <see cref="string"/> as an <see cref="XdslVersion"/>.
	/// </summary>
	/// <param name="version"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslVersion Parse(ReadOnlySpan<char> version)
	{
		if (version.Equals("Latest", StringComparison.OrdinalIgnoreCase)) {
			return OnePoint0;
		}
		
		int num = version.IndexOf('.');

		if (num < 0) {
			throw new ArgumentException("Version is too short", nameof(version));
		}

		var sMajor = version[..num];
		var sMinor = version[++num..];

		bool flag = int.TryParse(sMajor, out int major) & int.TryParse(sMinor, out int minor);

		if (!flag) {
			throw new ArgumentException("Version is not in the correct format", nameof(version));
		}

		return new XdslVersion(major, minor);
	}

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XdslVersion"/> objects are equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
	public static bool operator ==(XdslVersion left, XdslVersion right) => left.Equals(right);

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XdslVersion"/> objects are not equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
	public static bool operator !=(XdslVersion left, XdslVersion right) => !(left == right);
}