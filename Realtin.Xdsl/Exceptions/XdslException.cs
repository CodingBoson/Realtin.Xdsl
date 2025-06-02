using System;
using System.Runtime.Serialization;

namespace Realtin.Xdsl;

/// <summary>
/// Base class for all exceptions thrown by XDSL.
/// </summary>
public class XdslException : Exception
{
	/// <summary>
	/// Initialize a new instance of the <see cref="XdslException"/> class.
	/// </summary>
	public XdslException()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public XdslException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">
	/// The exception that is the cause of the current exception.
	/// </param>
	public XdslException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslException"/> class.
	/// </summary>
	/// <param name="info"></param>
	/// <param name="context"></param>
	protected XdslException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}