using System.Collections.Generic;

namespace Realtin.Xdsl.Serialization;

public class XdslLinkedListSerializer<T> : XdslSerializer<LinkedList<T>>
{
	public override string GetXName() => "LinkedList";

	public override void Serialize(XdslWriter writer, LinkedList<T>? value, XdslSerializerOptions options)
	{
		if (value == null) {
			return;
		}

		var itemName = options.NamingConvention.Apply("Item");

		foreach (var item in value) {
			writer.Write(itemName, SerializeToElement(item, options));
		}
	}

	public override LinkedList<T>? Deserialize(XdslReader reader, XdslSerializerOptions options)
	{
		if (!reader.CanRead()) {
			return default;
		}

		var list = new LinkedList<T>();

		for (int i = 0; i < reader.Length; i++) {
			// It's more performant to use XdslReader.GetProperty(int index) for collection deserialization.

			list.AddLast(Deserialize<T>(reader.GetProperty(i))!);
		}

		return list;
	}
}