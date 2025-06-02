using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

internal static partial class ObjectDefaultSerializer
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deserialize(ref object instance, XdslDocument document, Type type, XdslSerializerOptions options)
	{
		DeserializeObject(ref instance, document.Root!, type, false, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DeserializeObject(ref object instance, XdslElement xdslObject, Type type, bool serializeByRef, XdslSerializerOptions options)
	{
		if (xdslObject.IsEmpty) {
			return;
		}

		if (serializeByRef && xdslObject.TryGetAttribute("class", out var classAtt)) {
			type = options.TypeFinder.FindType(classAtt.Value) ?? type;
		}

		var typeInfo = XdslTypeInfo.Create(type, options);

		DeserializeObject(ref instance, xdslObject, typeInfo, serializeByRef, options);

		if (instance is IXdslSerializationCallbackReceiver receiver) {
			receiver.OnAfterDeserialize();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeserializeObject(ref object instance, XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		if (typeInfo.IsSerializable) {
			DeserializeSerializable(ref instance, xdslObject, options);
		}
		else if (typeInfo.IsList && typeInfo.IsValidCollection) {
			DeserializeList(ref instance, xdslObject, typeInfo, serializeByRef, options);
		}
		else if (typeInfo.IsDictionary && typeInfo.IsValidCollection) {
			DeserializeDictionary(ref instance, xdslObject, typeInfo, serializeByRef, options);
		}
		else if (typeInfo.IsDefaultType) {
			throw new XdslSerializerException($"Cannot deserialize to an existing instance of type {typeInfo.Type}.");
		}

		var properties = typeInfo.Properties;
		var canResolveTypes = instance is IXdslSerializeReferenceResolver;

		for (int i = 0; i < properties.Count; i++) {
			var property = properties[i];

			var xdslProperty = GetProperty(xdslObject, property, options);

			if (xdslProperty is null) {
				if (property.IsRequired) {
					throw new XdslSerializerException($"Required property {property.Name} on {typeInfo.Name} was not found.");
				}

				continue;
			}

			var propertyValue = DeserializeObject(xdslProperty, GetPropertyType(instance, property),
				property.SerializeByRef && !canResolveTypes, options);

			property.SetValue(target: instance, propertyValue,
				xdslProperty.HasAttribute(XdslAttributes.Encoding));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeSerializable(ref object instance, XdslElement xdslObject, XdslSerializerOptions options)
	{
		IXdslSerializable serializable = (IXdslSerializable)instance;

		var reader = XdslReader.Create(xdslObject);

		serializable.Deserialize(reader, options);

		return serializable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeserializeList(ref object instance, XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		if (instance is Array) {
			throw new XdslSerializerException($"Arrays are immutable.");
		}

		var xdslItems = xdslObject.Children;
		var elementType = typeInfo.ListElementType!;

		if (xdslItems is null) {
			return;
		}

		IList list = (IList)instance;

		list.Clear();

		for (var i = 0; i < xdslItems.Count; i++) {
			var xdslItem = xdslItems[i];

			var item = DeserializeObject(xdslItem, elementType, serializeByRef, options);

			list.Add(item);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeserializeDictionary(ref object instance, XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		var keyType = typeInfo.DictionaryKeyType!;
		var valueType = typeInfo.DictionaryValueType!;
		var xdslItems = xdslObject.Children;

		if (xdslItems is null) {
			return;
		}

		IDictionary dictionary = (IDictionary)instance;

		dictionary.Clear();

		var keyName = options.NamingConvention.Apply("Key");
		var valueName = options.NamingConvention.Apply("Value");
		var serializeKeyByRef = serializeByRef && !typeInfo.DoNotSerializeKeyByRef;
		var serializeValueByRef = serializeByRef && !typeInfo.DoNotSerializeValueByRef;

		for (int i = 0; i < xdslItems.Count; i++) {
			var xdslItem = xdslItems[i];

			var xdslKey = xdslItem.GetChild(keyName)
				?? throw new XdslSerializerException("Invalid dictionary format.");
			var xdslValue = xdslItem.GetChild(valueName)
				?? throw new XdslSerializerException("Invalid dictionary format.");

			var key = DeserializeObject(xdslKey, keyType, serializeKeyByRef, options);
			var value = DeserializeObject(xdslValue, valueType, serializeValueByRef, options);

			dictionary.Add(key, value);
		}
	}
}