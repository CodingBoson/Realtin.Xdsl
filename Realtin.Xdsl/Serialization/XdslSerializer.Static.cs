using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Serialization;

/// <summary>
/// Provides functionality to serialize objects or value types to XDSL and deserialize
/// XDSL into objects or value types.
/// </summary>
public abstract partial class XdslSerializer
{
	/// <summary>
	/// Use this message to warn others from using a serialization only constructor.
	/// </summary>
	public const string ConstructorWarning = "This parameterless constructor is only made public for serialization. Use another constructor instead.";

	/// <summary>
	/// Converts the value of a type specified by the <paramref name="type"/> parameter into an <see cref="XdslDocument"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslDocument Serialize(object? value, Type type, XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		try {
			return ObjectDefaultSerializer.Serialize(value, options);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslSerializerException($"Unexpected error while serializing {type}.", ex);
		}
	}

	/// <summary>
	/// Converts the value of a type specified by a generic type parameter into an <see cref="XdslDocument"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslDocument Serialize<T>([AllowNull] T value, XdslSerializerOptions? options = default)
	{
		return Serialize(value, typeof(T), options);
	}

	/// <summary>
	/// Converts the value of a type specified by the <paramref name="type"/> parameter into an XDSL <see cref="string"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	/// <param name="writeIndented"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string SerializeToString(object? value, Type type, bool writeIndented = false, XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		try {
			return ObjectDefaultSerializer.SerializeObject(value, false, options).WriteToString(writeIndented);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslSerializerException($"Unexpected error while serializing {type}.", ex);
		}
	}

	/// <summary>
	/// Converts the value of a type specified by a generic type parameter into an XDSL <see cref="string"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <param name="writeIndented"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string SerializeToString<T>([AllowNull] T value, bool writeIndented = false, XdslSerializerOptions? options = default)
	{
		return SerializeToString(value, typeof(T), writeIndented, options);
	}

	/// <summary>
	/// Converts the value of a type specified by the <paramref name="type"/> parameter into an <see cref="XdslElement"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslElement SerializeToElement(object? value, Type type, XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		try {
			return ObjectDefaultSerializer.SerializeObject(value, false, options);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslSerializerException($"Unexpected error while serializing {type}.", ex);
		}
	}

	/// <summary>
	/// Converts the value of a type specified by a generic type parameter into an <see cref="XdslElement"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslElement SerializeToElement<T>([AllowNull] T value, XdslSerializerOptions? options = default)
	{
		return SerializeToElement(value, typeof(T), options);
	}

	/// <summary>
	/// Converts the <see cref="XdslDocument"/> representing an XDSL object into an instance of type <paramref name="type"/>.
	/// </summary>
	/// <param name="document"></param>
	/// <param name="type"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object? Deserialize(XdslDocument document, Type type, XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		try {
			return ObjectDefaultSerializer.Deserialize(document, type, options);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslSerializerException($"Unexpected error while deserializing {type}.", ex);
		}
	}

	/// <summary>
	/// Converts the <see cref="XdslDocument"/> representing an XDSL object into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="document"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(XdslDocument document, XdslSerializerOptions? options = default)
	{
		return (T?)Deserialize(document, typeof(T), options);
	}

	/// <summary>
	/// Converts the <see cref="XdslElement"/> representing an XDSL object into an instance of type <paramref name="type"/>.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="type"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object? Deserialize(XdslElement element, Type type, XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		try {
			return ObjectDefaultSerializer.DeserializeObject(element, type, false, options);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslSerializerException($"Unexpected error while deserializing {type}.", ex);
		}
	}

	/// <summary>
	/// Converts the <see cref="XdslElement"/> representing an XDSL object into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(XdslElement element, XdslSerializerOptions? options = default)
	{
		return (T?)Deserialize(element, typeof(T), options);
	}

	/// <summary>
	/// Converts the <see cref="string"/> representing an XDSL object into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="xdsl"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(string xdsl, XdslSerializerOptions? options = default)
	{
		return (T?)Deserialize(CachedXdslDocumentLoader.Create(xdsl), typeof(T), options);
	}
}

public abstract partial class XdslSerializer
{
	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="document"></param>
	/// <param name="options"></param>
	public static void Deserialize(ref object instance, XdslDocument document, XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(instance), instance);

		options ??= XdslSerializerOptions.Default;

		ObjectDefaultSerializer.Deserialize(ref instance, document, instance.GetType(), options);
	}

	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="instance"></param>
	/// <param name="document"></param>
	/// <param name="options"></param>
	public static void Deserialize<T>([DisallowNull] ref T instance, XdslDocument document, XdslSerializerOptions? options = default)
	{
		object obj = instance!;

		Deserialize(ref obj, document, options);
	}

	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="element"></param>
	/// <param name="options"></param>
	public static void Deserialize(ref object instance, XdslElement element, XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(instance), instance);

		options ??= XdslSerializerOptions.Default;

		ObjectDefaultSerializer.DeserializeObject(ref instance, element, instance.GetType(), false, options);
	}

	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="instance"></param>
	/// <param name="element"></param>
	/// <param name="options"></param>
	public static void Deserialize<T>([DisallowNull] ref T instance, XdslElement element, XdslSerializerOptions? options = default)
	{
		object obj = instance!;

		Deserialize(ref obj, element, options);
	}

	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="xdsl"></param>
	/// <param name="options"></param>
	public static void Deserialize(ref object instance, string xdsl, XdslSerializerOptions? options = default)
	{
		Deserialize(ref instance, CachedXdslDocumentLoader.Create(xdsl), options);
	}

	/// <summary>
	/// Deserialize to an existing instance.
	/// <para>Note: Arrays and built in types are immutable
	/// and cannot be deserialized by this method.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="instance"></param>
	/// <param name="xdsl"></param>
	/// <param name="options"></param>
	public static void Deserialize<T>([DisallowNull] ref T instance, string xdsl, XdslSerializerOptions? options = default)
	{
		Deserialize(ref instance, CachedXdslDocumentLoader.Create(xdsl), options);
	}
}

public abstract partial class XdslSerializer
{
	/// <summary>
	/// Converts the value of a type specified by the <typeparamref name="T"/> parameter into an <see cref="XdslDocument"/> using the provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="serializer"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslDocument Serialize<T>(T? value, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(serializer), serializer);

		using var writer = XdslWriter.Create(serializer.GetXName());

		SerializeImpl(writer, value, serializer, options);

		return writer.ToDocument();
	}

	/// <summary>
	/// Converts the value of a type specified by the <typeparamref name="T"/> parameter into an XDSL <see cref="string"/> using the provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="serializer"></param>
	/// <param name="writeIndented"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string SerializeToString<T>(T? value, XdslSerializer<T> serializer,
		bool writeIndented = false, XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(serializer), serializer);

		using var writer = XdslWriter.Create(serializer.GetXName());

		SerializeImpl(writer, value, serializer, options);

		return writer.AsElement().WriteToString(writeIndented);
	}

	/// <summary>
	/// Converts the value of a type specified by the <typeparamref name="T"/> parameter into an <see cref="XdslElement"/> using the provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="serializer"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XdslElement SerializeToElement<T>(T? value, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(serializer), serializer);

		using var writer = XdslWriter.Create(serializer.GetXName());

		SerializeImpl(writer, value, serializer, options);

		return writer.AsElement();
	}

	/// <summary>
	/// Converts the <see cref="XdslDocument"/> representing an XDSL object into an instance of type <typeparamref name="T"/> using provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="document"></param>
	/// <param name="serializer"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(XdslDocument document, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(serializer), serializer);

		options ??= XdslSerializerOptions.Default;

		using var reader = XdslReader.Create(document);

		return serializer.Deserialize(reader, options);
	}

	/// <summary>
	/// Converts the <see cref="XdslElement"/> representing an XDSL object into an instance of type <typeparamref name="T"/> using provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="serializer"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(XdslElement element, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(serializer), serializer);

		options ??= XdslSerializerOptions.Default;

		using var reader = XdslReader.Create(element);

		return serializer.Deserialize(reader, options);
	}

	/// <summary>
	/// Converts the <see cref="string"/> representing an XDSL object into an instance of type <typeparamref name="T"/> using provided <paramref name="serializer"/>.
	/// </summary>
	/// <param name="xdsl"></param>
	/// <param name="serializer"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="XdslSerializerException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? Deserialize<T>(string xdsl, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		return Deserialize(CachedXdslDocumentLoader.Create(xdsl), serializer, options);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SerializeImpl<T>(XdslWriter writer, T? value, XdslSerializer<T> serializer,
		XdslSerializerOptions? options = default)
	{
		options ??= XdslSerializerOptions.Default;

		serializer.Serialize(writer, value, options);
	}
}