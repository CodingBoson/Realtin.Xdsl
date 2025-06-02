using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl;

/// <summary>
/// Provides a <see langword="base"/> <see langword="class"/> for strongly typed elements in XDSL.
/// See also <see cref="StronglyTypedElementExtensions.AsStronglyTyped{T}(XdslElement)"/>
/// </summary>
public abstract class StronglyTypedElement
{
	public readonly struct DataWriter(StronglyTypedElement owner)
	{
		public readonly StronglyTypedElement Owner = owner;

		/// <summary>
		/// Writes the specified <paramref name="value"/> to an element specified by <paramref name="elementName"/>.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <exception cref="XdslException">
		/// Element '<paramref name="elementName"/>' was not found. Make sure to perform schema validation.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string elementName, string value)
		{
			var child = Owner.Element.GetChild(elementName)
				?? throw new XdslException($"Element '{elementName}' was not found. Make sure to perform schema validation.");

			child.Text = value;
		}

		/// <summary>
		/// Writes the specified <paramref name="value"/> to an element specified by <paramref name="elementName"/>.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <exception cref="XdslException">
		/// Element '<paramref name="elementName"/>' was not found. Make sure to perform schema validation.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string elementName, bool value) => Write(elementName, value.ToString());

		/// <summary>
		/// Writes the specified <paramref name="value"/> to an element specified by <paramref name="elementName"/>.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <exception cref="XdslException">
		/// Element '<paramref name="elementName"/>' was not found. Make sure to perform schema validation.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string elementName, int value) => Write(elementName, value.ToString());

		/// <summary>
		/// Writes the specified <paramref name="value"/> to an element specified by <paramref name="elementName"/>.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <exception cref="XdslException">
		/// Element '<paramref name="elementName"/>' was not found. Make sure to perform schema validation.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string elementName, float value) => Write(elementName, value.ToString());
	}

	protected struct ElementValidator(XdslElement element)
	{
		private List<string>? _missingElements = null;

		private readonly XdslElement Element = element;

		/// <summary>
		/// Requires that an element specified by the <paramref name="elementName"/> is present in the <see cref="Element"/>.
		/// </summary>
		/// <param name="elementName"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RequireElement(string elementName)
		{
			if (!Element.HasChild(elementName)) {
				_missingElements ??= [];

				_missingElements.Add(elementName);
			}
		}

		/// <summary>
		/// Throws a <see cref="StronglyTypedException"/> if any of the required elements were not found.
		/// </summary>
		/// <exception cref="StronglyTypedException"></exception>
		public readonly void ThrowIfHasMissingElements()
		{
			if (_missingElements != null) {
				throw new StronglyTypedException($"The specified element failed to bind to this strongly typed schema. Missing Elements '{string.Join(", ", _missingElements)}'");
			}
		}
	}

	private XdslReader? _reader;

	/// <summary>
	/// Gets the element represented by this strongly typed object.
	/// </summary>
	public XdslElement Element { get; internal set; } = default!;

	/// <summary>
	/// Gets an <see cref="XdslReader"/> for the represented element.
	/// </summary>
	public XdslReader Reader => _reader ??= XdslReader.Create(Element);

	/// <summary>
	/// Gets an <see cref="DataWriter"/> for the underlying element.
	/// </summary>
	public DataWriter Writer => new(this);

	/// <summary>
	/// Override this method to implement custom logic for schema validation.
	/// </summary>
	protected virtual void ValidateSchema()
	{
	}

	internal void ValidateSchemaInternal() => ValidateSchema();
}