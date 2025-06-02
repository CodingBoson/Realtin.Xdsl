using System;

namespace Realtin.Xdsl.Serialization;

public class XdslSerializerException : XdslException
{
	private const string _defaultMessage = "";

	public XdslSerializerException()
		: base(_defaultMessage) { }

	public XdslSerializerException(string message)
		: base(message) { }

	public XdslSerializerException(string message, Exception innerException)
		: base(message, innerException) { }

	public XdslSerializerException(Exception innerException)
		: base(_defaultMessage, innerException) { }
}