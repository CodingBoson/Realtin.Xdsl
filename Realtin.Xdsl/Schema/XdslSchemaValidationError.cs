using System;
using System.Diagnostics.CodeAnalysis;

namespace Realtin.Xdsl.Schema;

/// <summary>
/// Represents a schema validation error.
/// </summary>
/// <param name="errorType"></param>
/// <param name="message"></param>
public readonly struct XdslSchemaValidationError(XdslSchemaErrorType errorType, string message) : IEquatable<XdslSchemaValidationError>
{
	/// <summary>
	/// The type of error this is.
	/// </summary>
	public XdslSchemaErrorType ErrorType { get; } = errorType;

	/// <summary>
	/// The message for this <see cref="XdslSchemaValidationError"/>.
	/// </summary>
	public string Message { get; } = message;

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) 
        => obj is XdslSchemaValidationError error && Equals(error);

    /// <inheritdoc/>
    public bool Equals(XdslSchemaValidationError other) 
        => ErrorType == other.ErrorType && Message == other.Message;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ErrorType, Message);

    /// <inheritdoc/>
    public override string ToString() => $"ErrorType: {ErrorType}, Message: {Message}";

    /// <summary>
    /// Returns a value that indicates whether two <see cref="XdslSchemaValidationError"/> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
    /// are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(XdslSchemaValidationError left, XdslSchemaValidationError right) 
        => left.Equals(right);

    /// <summary>
    /// Returns a value that indicates whether two <see cref="XdslSchemaValidationError"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/>
    /// are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(XdslSchemaValidationError left, XdslSchemaValidationError right) 
        => !(left == right);
}