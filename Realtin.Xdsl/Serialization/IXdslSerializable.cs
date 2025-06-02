namespace Realtin.Xdsl.Serialization;

public interface IXdslSerializable
{
	void Serialize(XdslWriter writer, XdslSerializerOptions options);

	void Deserialize(XdslReader reader, XdslSerializerOptions options);
}