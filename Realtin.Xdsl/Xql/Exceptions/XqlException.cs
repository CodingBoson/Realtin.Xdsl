using System;

namespace Realtin.Xdsl.Xql;

/// <summary>
/// Provides the exception thrown when an error occurs while processing an XQL expression.
/// </summary>
public sealed class XqlException : XdslException
{
	/// <summary>
	/// Initialize a new instance of the <see cref="XqlException"/> class.
	/// </summary>
	public XqlException()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XqlException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public XqlException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XqlException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">
	/// The exception that is the cause of the current exception.
	/// </param>
	public XqlException(string message, Exception innerException) : base(message, innerException)
	{
	}
}