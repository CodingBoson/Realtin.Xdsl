using System;

namespace Realtin.Xdsl.Serialization
{
    public interface IXdslSerializeReferenceResolver
	{
		Type GetType(string propertyName);
	}
}