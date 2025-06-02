using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

internal static partial class ObjectDefaultSerializer
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslDocument Serialize(object? obj, XdslSerializerOptions options)
	{
		var document = new XdslDocument();

		var objectGraph = SerializeObject(obj, false, options);

		document.AppendChild(objectGraph);

		return document;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslElement SerializeObject(object? obj, bool serializeByRef, XdslSerializerOptions options, string? name = default, string? encoding = default)
	{
		if (obj == null) {
			return NullElement(name ?? "Object");
		}

		if (obj is IXdslSerializationCallbackReceiver receiver) {
			receiver.OnBeforeSerialize();
		}

		var type = obj.GetType();

		var typeInfo = XdslTypeInfo.Create(type, options);

		var element = SerializeObject(typeInfo, obj, serializeByRef, options);

		name ??= typeInfo.Name;

		element.Name = options.NamingConvention.Apply(name);

		if (serializeByRef) {
			element.AddAttribute(typeInfo.ClassAttribute);
		}

		if (!string.IsNullOrEmpty(encoding)) {
			element.AddAttribute(XdslAttributes.Encoding, encoding);
		}

		return element;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeObject(XdslTypeInfo typeInfo, object obj, bool serializeByRef, XdslSerializerOptions options)
	{
		var type = typeInfo.Type;
		var serializer = options.Serializers.GetSerializer(type);

		if (serializer != null) {
			return SerializeWithSerializer(serializer, obj, options);
		}
		else if (options.TryGetConverter(type, out var converter)) {
			return SerializeWithConverter(converter, obj);
		}
		else if (typeInfo.IsDefaultType) {
			return SerializeDefaultType(obj, options);
		}
		else if (obj is IXdslSerializable serializable) {
			return SerializeSerializable(serializable, options);
		}
		else if (typeInfo.IsList && typeInfo.IsValidCollection) {
			return SerializeList((IList)obj, serializeByRef, options);
		}
		else if (typeInfo.IsDictionary && typeInfo.IsValidCollection) {
			return SerializeDictionary(typeInfo, (IDictionary)obj, serializeByRef, options);
		}

		var xdslObject = new XdslElement("");

		if (!typeInfo.CanCreateInstance) {
			return xdslObject;
		}

		var properties = typeInfo.Properties;
		var canResolveTypes = obj is IXdslSerializeReferenceResolver;

		for (int i = 0; i < properties.Count; i++) {
			var property = properties[i];

			var value = property.GetValue(obj);

			var xdslProperty = SerializeObject(value,
					property.SerializeByRef && !canResolveTypes, options,
					property.Name, property.Encoding);

			xdslObject.AppendChild(xdslProperty);
		}

		return xdslObject;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeWithSerializer(XdslSerializer serializer, object value, XdslSerializerOptions options)
	{
		var writer = XdslWriter.Create();

		serializer.Serialize(writer, value, options);

		return writer.AsElement();
	}

	private static XdslElement SerializeWithConverter(IXdslConverter converter, object value)
	{
		var element = new XdslElement("");

		converter.Serialize(new XdslValue(element), value);

		return element;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeDefaultType(object obj, XdslSerializerOptions options)
	{
		return new XdslElement("", DefaultTypeSerializer.Serialize(obj, options));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeSerializable(IXdslSerializable serializable, XdslSerializerOptions options)
	{
		var writer = XdslWriter.Create();

		serializable.Serialize(writer, options);

		return writer.AsElement();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeList(IList list, bool serializeByRef,
		XdslSerializerOptions options)
	{
		if (list.Count == 0) {
			return EmptyCollection();
		}

		var xdslList = new XdslElement("");

		var itemName = options.NamingConvention.Apply("Item");

		for (int i = 0; i < list.Count; i++) {
			var item = list[i];

			var xdslItem = SerializeObject(item, serializeByRef, options, itemName);

			xdslList.AppendChild(xdslItem);
		}

		return xdslList;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement SerializeDictionary(XdslTypeInfo typeInfo, IDictionary dictionary,
		bool serializeByRef, XdslSerializerOptions options)
	{
		if (dictionary.Count == 0) {
			return EmptyCollection();
		}

		var xdslDictionary = new XdslElement("");

		var enumerator = dictionary.GetEnumerator();

		var itemName = options.NamingConvention.Apply("Item");
		var keyName = options.NamingConvention.Apply("Key");
		var valueName = options.NamingConvention.Apply("Value");
		var serializeKeyByRef = serializeByRef && !typeInfo.DoNotSerializeKeyByRef;
		var serializeValueByRef = serializeByRef && !typeInfo.DoNotSerializeValueByRef;

		while (enumerator.MoveNext()) {
			var key = enumerator.Key;
			var value = enumerator.Value;

			var xdslKey = SerializeObject(key, serializeKeyByRef, options, keyName);
			var xdslValue = SerializeObject(value, serializeValueByRef, options, valueName);

			var xdslElement = new XdslElement(itemName);

			xdslElement.AppendChild(xdslKey);
			xdslElement.AppendChild(xdslValue);

			xdslDictionary.AppendChild(xdslElement);
		}

		if (enumerator is IDisposable disposable) {
			disposable.Dispose();
		}

		return xdslDictionary;
	}
}