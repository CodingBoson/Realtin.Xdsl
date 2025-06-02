using System;
using System.Diagnostics;

namespace Realtin.Xdsl;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StronglyTypedException<T> : StronglyTypedException
{
	public StronglyTypedException() : base($"{typeof(T)}: Failed to bind to the specified element.")
	{
	}

	public StronglyTypedException(string message) : base($"{typeof(T)}: {message}")
	{
	}

	public StronglyTypedException(string message, Exception innerException) : base($"{typeof(T)}: {message}", innerException)
	{
	}

	private string GetDebuggerDisplay() => $"{nameof(StronglyTypedException)}<{typeof(T).Name}>";
}