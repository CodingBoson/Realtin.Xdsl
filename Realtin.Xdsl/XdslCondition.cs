using System;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL condition used by the <see cref="XdslConditionalElementProcessor"/> class.
/// </summary>
/// <param name="name"></param>
/// <param name="isChecked"></param>
public sealed class XdslCondition(string name, bool isChecked) : IEquatable<XdslCondition>
{
	/// <summary>
	/// The name of this condition.
	/// </summary>
	public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

	/// <summary>
	/// Is this condition true?
	/// </summary>
	public bool IsChecked { get; } = isChecked;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is XdslCondition condition && Equals(condition);

    /// <inheritdoc/>
    public bool Equals(XdslCondition? other) => other is not null 
        && Name == other.Name 
        && IsChecked == other.IsChecked;
	
    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name, IsChecked);

    /// <inheritdoc/>
    public override string ToString() => $"{Name}, {IsChecked}";

    /// <summary>
    /// Indicates whether two <see cref="XdslCondition"/> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if left is equal to right; otherwise, false.</returns>
    public static bool operator ==(XdslCondition? left, XdslCondition? right)
	{
		if (ReferenceEquals(left, right)) {
			return true;
		}

		return left is not null && left.Equals(right);
	}

    /// <summary>
    /// Indicates whether two <see cref="XdslCondition"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if left is not equal to right; otherwise, false.</returns>
    public static bool operator !=(XdslCondition? left, XdslCondition? right) => !(left == right);
}