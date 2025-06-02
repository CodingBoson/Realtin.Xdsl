using System;

namespace Realtin.Xdsl.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public abstract class XdslSerializerAttribute : Attribute
{
	protected XdslSerializerAttribute()
	{
	}
}