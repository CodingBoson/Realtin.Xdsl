using System;
using System.Collections;
using System.Collections.Generic;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Serialization;

public sealed class XdslTypeInfo
{
	private static readonly Dictionary<Type, XdslTypeInfo> _cache = [];

	public readonly Type Type;

	public readonly XdslTypeInfoType InfoType;

	public readonly string Name;

	public bool IsList => InfoType == XdslTypeInfoType.List;

	public bool IsDictionary => InfoType == XdslTypeInfoType.Dictionary;

	public bool IsDefaultType => InfoType == XdslTypeInfoType.DefaultType;

	public readonly bool IsValidCollection;

	public readonly bool IsSerializable;

	public readonly bool CanCreateInstance;

	public readonly Type? ListElementType;

	public readonly Type? DictionaryKeyType;

	public readonly Type? DictionaryValueType;

	public readonly bool DoNotSerializeKeyByRef;

	public readonly bool DoNotSerializeValueByRef;

	public readonly IList<XdslPropertyInfo> Properties;

	public readonly XdslAttribute ClassAttribute;

	public readonly DefaultActivator Activator;

	private XdslTypeInfo(Type type, XdslSerializerOptions options)
	{
		Type = type;
		Name = type.LegalXdslName(false);
		IsSerializable = typeof(IXdslSerializable).IsAssignableFrom(type);
		ClassAttribute = new XdslAttribute(XdslAttributes.Class, options.TypeFinder.GetTypeId(type));
		Activator = DefaultActivator.Create(type);
		CanCreateInstance = !type.IsInterface && !type.IsAbstract;
		var serializeCollectionAsObject = type.HasAttribute<XdslSerializeAsObjectAttribute>();

		if (!serializeCollectionAsObject && typeof(IList).IsAssignableFrom(type)) {
			Properties = Array.Empty<XdslPropertyInfo>();
			InfoType = XdslTypeInfoType.List;
			ListElementType = GetListElementType(type);
			IsValidCollection = ListElementType != null;
		}
		else if (!serializeCollectionAsObject && typeof(IDictionary).IsAssignableFrom(type)) {
			Properties = Array.Empty<XdslPropertyInfo>();
			InfoType = XdslTypeInfoType.Dictionary;

			GetDictionaryKeyValueType(type, out var KeyType, out var ValueType);

			DictionaryKeyType = KeyType;
			DictionaryValueType = ValueType;

			IsValidCollection = KeyType != null && ValueType != null;

			if (IsValidCollection) {
				DoNotSerializeKeyByRef = KeyType!.IsSealed;
				DoNotSerializeValueByRef = ValueType!.IsSealed;
			}
		}
		else if (!DefaultTypeSerializer.CanSerialize(type)) {
			InfoType = XdslTypeInfoType.Object;

			Properties = type.GetSerializationProperties(options);
		}
		else {
			Properties = Array.Empty<XdslPropertyInfo>();
			InfoType = XdslTypeInfoType.DefaultType;
		}

		if (InfoType == XdslTypeInfoType.Object) {
			SortProperties();
		}
	}

	private void SortProperties()
	{
		Array.Sort((XdslPropertyInfo[])Properties, (x, y) => {
			var xOrder = x.UnderlyingMember.
				GetAttributeValue<XdslMemberOrderAttribute, int>(x => x.Order, 10);
			var yOrder = y.UnderlyingMember.
				GetAttributeValue<XdslMemberOrderAttribute, int>(x => x.Order, 10);

			return xOrder.CompareTo(yOrder);
		});
	}

    public static void Cache(Type type, XdslSerializerOptions options) => Create(type, options);

    public static bool ClearCacheForType(Type type) => _cache.Remove(type);

    public static XdslTypeInfo Create(Type type, XdslSerializerOptions options)
	{
		if (!_cache.TryGetValue(type, out var info)) {
			info = new XdslTypeInfo(type, options);

			_cache.Add(type, info);
		}

		return info;
	}

	private static Type? GetListElementType(Type type)
	{
		if (type.IsArray) {
			return type.GetElementType();
		}

		var interfaces = type.FindInterfaces((x, _) => typeof(IEnumerable).
		IsAssignableFrom(x), filterCriteria: null);

		foreach (var @interface in interfaces) {
			var genericParameters = @interface.GetGenericArguments();

			if (genericParameters.Length > 0) {
				return genericParameters[0];
			}
		}

		return null;
	}

	private static void GetDictionaryKeyValueType(Type type, out Type? keyType, out Type? valueType)
	{
		var interfaces = type.FindInterfaces((x, _) => typeof(IEnumerable).
		IsAssignableFrom(x), filterCriteria: null);
		foreach (var @interface in interfaces) {
			var genericParameters = @interface.GetGenericArguments();

			if (genericParameters.Length > 1) {
				keyType = genericParameters[0];
				valueType = genericParameters[1];

				return;
			}
		}

		keyType = valueType = null;
	}
}