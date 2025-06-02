using System;

namespace Realtin.Xdsl.Serialization;

internal sealed class DefaultTypeFinder : ITypeFinder
{
	public string GetTypeId(Type type)
	{
		return type.AssemblyQualifiedName;
	}

	public Type? FindType(string typeName)
	{
		return Type.GetType(typeName);
	}
}