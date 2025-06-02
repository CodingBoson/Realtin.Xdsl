using System;

namespace Realtin.Xdsl;

/// <summary>
/// Provides an <see langword="interface"/> for cloneable objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICloneable<T>
{
	/// <summary>
	/// Clones this instance.
	/// </summary>
	/// <returns></returns>
	T Clone();

	/// <summary>
	/// Clones this instance.
	/// </summary>
	/// <param name="with"></param>
	/// <returns></returns>
	T Clone(Action<T> with);
}