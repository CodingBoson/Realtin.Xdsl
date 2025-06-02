using System;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

/// <summary>
/// Provides extension methods for working with strongly typed element.
/// </summary>
public static class StronglyTypedElementExtensions
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="element"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="StronglyTypedException"></exception>
	public static T AsStronglyTyped<T>(this XdslElement element) where T : StronglyTypedElement, new()
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(element), element);

		var typedElement = new T {
			Element = element
		};

		typedElement.ValidateSchemaInternal();

		return typedElement;
	}
}