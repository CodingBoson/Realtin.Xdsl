using System;
using Realtin.Xdsl.Serialization;
using Realtin.Xdsl.Syntax;

namespace Realtin.Xdsl;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class XdslNameAttribute : XdslSerializerAttribute
{
	public string Name { get; }

	public XdslNameAttribute(string name)
	{
		SyntaxHelper.AssertValidName(name);

		Name = name;
	}
}