using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Realtin.Xdsl.Schema;

/// <summary>
/// Represents a validation result returned by <see cref="XdslSchema.Validate(XdslDocument)"/>
/// </summary>
/// <param name="success"></param>
/// <param name="errors"></param>
public readonly struct XdslSchemaValidationResult(bool success, XdslSchemaValidationError[] errors)
	: IEquatable<XdslSchemaValidationResult>
{
	/// <summary>
	/// Returns a value that indicates whether the validation process succeeded.
	/// </summary>
	public bool Success { get; } = success;

	/// <summary>
	/// Returns a value that indicates whether the validation process failed with errors.
	/// </summary>
	public bool HasErrors => !Success && Errors.Length > 0;

	/// <summary>
	/// Gets a list of errors.
	/// </summary>
	public XdslSchemaValidationError[] Errors { get; } = errors;

	/// <summary>
	/// Makes an error string.
	/// </summary>
	/// <returns>An error <see langword="string"/>.</returns>
	public string MakeErrorString()
	{
		if (Success) {
			return string.Empty;
		}

		return string.Join(Environment.NewLine, Errors);
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is XdslSchemaValidationResult result && Equals(result);
	}

	/// <inheritdoc/>
	public bool Equals(XdslSchemaValidationResult other) => Success == other.Success 
		&& HasErrors == other.HasErrors 
		&& EqualityComparer<XdslSchemaValidationError[]?>.Default.Equals(Errors, other.Errors);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Success, HasErrors, Errors);

    /// <summary>
    /// Determines whether two specified instances of
    /// <see cref="XdslSchemaValidationResult"/> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
    public static bool operator ==(XdslSchemaValidationResult left, XdslSchemaValidationResult right) 
		=> left.Equals(right);

    /// <summary>
    /// Determines whether two specified instances of
    /// <see cref="XdslSchemaValidationResult"/> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are
    /// not equal; otherwise, false.</returns>
    public static bool operator !=(XdslSchemaValidationResult left, XdslSchemaValidationResult right) 
		=> !(left == right);
}