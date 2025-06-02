using System;

namespace Realtin.Xdsl;

public class StronglyTypedException : XdslException
{
	public StronglyTypedException()
	{
	}

	public StronglyTypedException(string message) : base(message)
	{
	}

	public StronglyTypedException(string message, Exception innerException) : base(message, innerException)
	{
	}
}