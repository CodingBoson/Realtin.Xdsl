using System;

namespace Realtin.Xdsl.Serialization;

public abstract class XdslActivator
{
	public abstract bool CanCreateInstance(Type type);

	public abstract object CreateInstance(Type type, XdslSerializerOptions options);
}