using System;
using System.Diagnostics.CodeAnalysis;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL attribute.
/// Valid and default values for the attribute are defined in a schema.
/// </summary>
public sealed class XdslAttribute : IEquatable<XdslAttribute?>
{
	/// <summary>
	/// The name of this attribute.
	/// <para>Note: Names are case insensitive.</para>
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The value of this attribute.
	/// </summary>
	public string Value { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="XdslAttribute"/> class.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public XdslAttribute(string name, string value)
	{
		if (string.IsNullOrEmpty(name)) {
			throw new ArgumentNullException("name");
		}

		Name = name;
		Value = value;
	}

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as XdslAttribute);

    /// <inheritdoc/>
    public bool Equals([NotNullWhen(true)] XdslAttribute? other)
	{
		return other is not null &&
			   Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
			   Value == other.Value;
	}

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name, Value);

    /// <inheritdoc/>
    public override string ToString() => $"{Name}=\"{Value}\"";

    /// <summary>
    /// Indicates whether two <see cref="XdslAttribute"/> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if left is equal to right; otherwise, false.</returns>
    public static bool operator ==(XdslAttribute? left, XdslAttribute? right)
	{
		if (ReferenceEquals(left, right)) {
			return true;
		}

		return left is not null && right is not null && left.Equals(right);
	}

    /// <summary>
    /// Indicates whether two <see cref="XdslAttribute"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if left is not equal to right; otherwise, false.</returns>
    public static bool operator !=(XdslAttribute? left, XdslAttribute? right) => !(left == right);
}