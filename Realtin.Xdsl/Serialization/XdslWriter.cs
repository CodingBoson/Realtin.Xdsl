using System;

namespace Realtin.Xdsl.Serialization;

public sealed class XdslWriter : IDisposable
{
	private readonly XdslElement _root;

	private XdslElement _current;

	private int _propertyDepth;

	private XdslWriter(string name)
	{
		_root = new XdslElement(name);
		_current = _root;
		_propertyDepth = 0;
	}

	public XdslElement Write(string propertyName, bool value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, byte value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, sbyte value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, short value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, ushort value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, int value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, uint value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, long value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, ulong value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, float value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, double value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, decimal value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, char value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, string? value)
	{
		var elem = new XdslElement(propertyName, value);

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, DateTime value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, DateTimeOffset value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, Guid value)
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write<TEnum>(string propertyName, TEnum value)
		where TEnum : Enum
	{
		var elem = new XdslElement(propertyName, value.ToString());

		_current.AppendChild(elem);

		return elem;
	}

	public XdslElement Write(string propertyName, XdslElement property)
	{
		property.Name = propertyName;

		_current.AppendChild(property);

		return property;
	}

	public XdslElement StartProperty(string propertyName)
	{
		var property = new XdslElement(propertyName);

		_current.AppendChild(property);

		_propertyDepth++;

		_current = property;

		return property;
	}

	public void EndProperty()
	{
		if (_propertyDepth == 0) {
			throw new InvalidOperationException("EndProperty was called before StartProperty.");
		}

		_propertyDepth--;
		_current = (XdslElement)_current.Parent!;
	}

	public XdslDocument ToDocument()
	{
		var document = new XdslDocument();

		document.AppendChild(_root);

		return document;
	}

	public XdslElement AsElement()
	{
		return _root;
	}

	// Might Be Useful.
	public void Dispose()
	{
	}

	public static XdslWriter Create()
	{
		return new XdslWriter("Object");
	}

	public static XdslWriter Create(string xName)
	{
		return new XdslWriter(xName);
	}
}