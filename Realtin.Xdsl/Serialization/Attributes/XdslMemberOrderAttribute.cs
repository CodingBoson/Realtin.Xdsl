namespace Realtin.Xdsl.Serialization;

public sealed class XdslMemberOrderAttribute(int order) : XdslSerializerAttribute
{
	public int Order { get; } = order;
}