using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

internal static partial class ObjectDefaultSerializer
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object? Deserialize(XdslDocument document, Type type, XdslSerializerOptions options)
	{
		if (document.Root is null) {
			throw new XdslException("XdslDocument does not have a root element.");
		}

		return DeserializeObject(document.Root!, type, false, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object? DeserializeObject(XdslElement xdslObject, Type type, bool serializeByRef, XdslSerializerOptions options)
	{
		if (xdslObject.IsEmpty) {
			return null;
		}

		if (serializeByRef && xdslObject.TryGetAttribute("class", out var classAtt)) {
			type = options.TypeFinder.FindType(classAtt.Value) ?? type;
		}

		var typeInfo = XdslTypeInfo.Create(type, options);

		var value = DeserializeObject(xdslObject, typeInfo, serializeByRef, options);

		if (value is IXdslSerializationCallbackReceiver receiver) {
			receiver.OnAfterDeserialize();
		}

		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeObject(XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		var type = typeInfo.Type;
		var serializer = options.Serializers.GetSerializer(type);

		if (serializer != null) {
			return DeserializeWithSerializer(xdslObject, typeInfo, serializer, options);
		}
		else if (options.TryGetConverter(type, out var converter)) {
			return DeserializeWithConverter(xdslObject, type, converter);
		}
		else if (typeInfo.IsSerializable) {
			return DeserializeSerializable(xdslObject, typeInfo, options);
		}
		else if (typeInfo.IsDefaultType) {
			return DeserializeDefaultType(xdslObject, typeInfo, options);
		}
		else if (typeInfo.IsList && typeInfo.IsValidCollection) {
			return DeserializeList(xdslObject, typeInfo, serializeByRef, options);
		}
		else if (typeInfo.IsDictionary && typeInfo.IsValidCollection) {
			return DeserializeDictionary(xdslObject, typeInfo, serializeByRef, options);
		}

		if (!typeInfo.CanCreateInstance) {
			return null;
		}

		var obj = typeInfo.Activator.CreateInstance();
		var canResolveTypes = obj is IXdslSerializeReferenceResolver;

		var properties = typeInfo.Properties;

		for (int i = 0; i < properties.Count; i++) {
			var property = properties[i];

			var xdslProperty = GetProperty(xdslObject, property, options);

			if (xdslProperty is null) {
				if (property.IsRequired) {
					throw new XdslSerializerException($"Required property {property.Name} on {typeInfo.Name} was not found.");
				}

				continue;
			}

			var propertyValue = DeserializeObject(xdslProperty, GetPropertyType(obj, property),
				property.SerializeByRef && !canResolveTypes, options);

			property.SetValue(target: obj, propertyValue,
				xdslProperty.HasAttribute(XdslAttributes.Encoding));
		}

		return obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeWithConverter(XdslElement xdslObject, Type type, IXdslConverter converter)
	{
		return converter.Deserialize(new XdslValue(xdslObject), type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeWithSerializer(XdslElement xdslObject, XdslTypeInfo typeInfo, XdslSerializer serializer, XdslSerializerOptions options)
	{
		var reader = XdslReader.Create(xdslObject);

		return serializer.Deserialize(reader, typeInfo.Type, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeDefaultType(XdslElement xdslObject, XdslTypeInfo typeInfo, XdslSerializerOptions options)
	{
		return DefaultTypeSerializer.Deserialize(typeInfo.Type, xdslObject.Text, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeSerializable(XdslElement xdslObject, XdslTypeInfo typeInfo, XdslSerializerOptions options)
	{
		IXdslSerializable serializable = (IXdslSerializable)typeInfo.Activator.CreateInstance();

		var reader = XdslReader.Create(xdslObject);

		serializable.Deserialize(reader, options);

		return serializable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeList(XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		var xdslItems = xdslObject.Children;
		var type = typeInfo.Type;
		var elementType = typeInfo.ListElementType!;
		var isArray = type.IsArray;

		if (xdslItems is null) {
			return isArray
				? Array.CreateInstance(elementType, 0)
				: typeInfo.Activator.CreateInstance();
		}

		IList list = isArray
				? Array.CreateInstance(elementType, xdslItems.Count)
				: (IList)typeInfo.Activator.CreateInstance();

		for (var i = 0; i < xdslItems.Count; i++) {
			var xdslItem = xdslItems[i];

			var item = DeserializeObject(xdslItem, elementType, serializeByRef, options);

			if (isArray) {
				list[i] = item;

				continue;
			}

			list.Add(item);
		}

		return list;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? DeserializeDictionary(XdslElement xdslObject, XdslTypeInfo typeInfo, bool serializeByRef, XdslSerializerOptions options)
	{
		var keyType = typeInfo.DictionaryKeyType!;
		var valueType = typeInfo.DictionaryValueType!;
		var xdslItems = xdslObject.Children;

		if (xdslItems is null) {
			return typeInfo.Activator.CreateInstance();
		}

		IDictionary dictionary = (IDictionary)typeInfo.Activator.CreateInstance();

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

		return dictionary;
	}
}