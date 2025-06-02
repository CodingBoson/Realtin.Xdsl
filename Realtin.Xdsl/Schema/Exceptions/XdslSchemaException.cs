using System;

namespace Realtin.Xdsl.Schema;

/// <summary>
/// Provides the exception thrown when an error occurs while processing an XDSL Schema.
/// </summary>
public sealed class XdslSchemaException : XdslException
{
	/// <summary>
	/// Initialize a new instance of the <see cref="XdslSchemaException"/> class.
	/// </summary>
	public XdslSchemaException()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslSchemaException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public XdslSchemaException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslSchemaException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">
	/// The exception that is the cause of the current exception.
	/// </param>
	public XdslSchemaException(string message, Exception innerException) : base(message, innerException)
	{
	}
}