using System;

namespace Realtin.Xdsl.Serialization;

public interface ITypeFinder
{
	string GetTypeId(Type type);

	/// <summary>
	/// Gets the <see cref="Type"/> with the specified name, performing a case-sensitive search.
	/// </summary>
	/// <param name="typeName"></param>
	/// <returns>
	/// The type with the specified name, if found; otherwise, null.
	/// </returns>
	Type? FindType(string typeName);
}