using System;
using System.Collections.Generic;
using Realtin.Xdsl.Linq;

namespace Realtin.Xdsl.Schema;

internal static class XdslSchemaImpl
{
	private static readonly XdslSchemaValidationError[] Empty = [];

	#region Validation

	internal static XdslSchemaValidationResult ValidateImpl(XdslElement schemaImpl, XdslElement element)
	{
		var success = true;
		List<XdslSchemaValidationError>? errors = null;

		foreach (var attribute in schemaImpl.
			Where(x => {
				if (x.HasAttribute("attribute")) {
					var impl = element.GetAttribute(x.Name);

					return impl == null && x.HasAttribute("required");
				}

				return false;
			})) {
			errors ??= [];

			errors.Add(MissingAttribute(attribute, schemaImpl));
			success = false;
		}

		foreach (var child in schemaImpl.
			Where(x => {
				if (x.HasAttribute("attribute") || x.HasAttribute("root")) {
					return false;
				}

				var impl = element.GetChild(x.Name);

				return impl == null && x.HasAttribute("required");
			})) {
			errors ??= [];

			errors.Add(MissingElement(child, schemaImpl));
			success = false;
		}

		if (element.Attributes is not null) {
			for (int i = 0; i < element.Attributes.Count; i++) {
				var attribute = element.Attributes[i];

				HandleAttribute(schemaImpl, ref errors, ref success, attribute);
			}
		}

		if (element.Children is not null) {
			for (int i = 0; i < element.Children.Count; i++) {
				var child = element.Children[i];

				if (child is XdslComment)
					continue;

				HandleElement(schemaImpl, ref errors, ref success, child);
			}
		}

		return new(success, errors?.ToArray() ?? Empty);
	}

	internal static void HandleAttribute(XdslElement schemaImpl, ref List<XdslSchemaValidationError>? errors, ref bool success, XdslAttribute attribute)
	{
		var impl = GetAttributeImpl(schemaImpl, attribute.Name);

		if (impl == null) {
			errors ??= [];
			errors.Add(UnknownAttribute(attribute, schemaImpl));
			success = false;
		}
		else {
			var type = impl.GetAttribute("type")?.Value ?? string.Empty;

			if (XdslTypeValidator.TypeValidators.TryGetValue(type, out var typeValidator)) {
				if (typeValidator.Validate(attribute.Value)) {
				}
				else {
					errors ??= [];
					errors.Add(InvalidType(attribute.Value, attribute.Name, type, "Element"));
					success = false;
				}
			}
		}
	}

	internal static void HandleElement(XdslElement schemaImpl, ref List<XdslSchemaValidationError>? errors, ref bool success, XdslElement child)
	{
		var impl = GetChildImpl(schemaImpl, child.Name);

		if (impl == null) {
			errors ??= [];
			errors.Add(UnknownElement(child, schemaImpl));
			success = false;
		}
		else {
			var type = impl.GetAttribute("type")?.Value ?? string.Empty;

			if (XdslTypeValidator.TypeValidators.TryGetValue(type, out var valueValidator)) {
				if (child.Children is null &&
					valueValidator.Validate(child.Text)) {
				}
				else {
					errors ??= [];
					errors.Add(InvalidType(child.Text, child.Name, type, "Element"));
					success = false;
				}
			}
			else {
				var noChildElements = impl.GetAttribute("noChildElements");

				if (noChildElements != null && child.Children is not null) {
					errors ??= [];
					errors.Add(DoesNotSupportChildElements(child));
					success = false;
				}
				else {
					var result = ValidateImpl(impl, child);

					if (result.HasErrors) {
						errors ??= [];

						foreach (var error in result.Errors!) {
							errors.Add(error);
						}

						success = false;
					}
				}
			}
		}
	}

	internal static XdslElement? GetRootImpl(XdslDocument schema, string name)
	{
		return schema.Root?.Find(x => {
			if (x.HasAttribute("root")) {
				return x.Name == name;
			}

			return false;
		});
	}

	internal static XdslElement? GetAttributeImpl(XdslElement element, string name)
	{
		return element.Find(x => {
			if (x.HasAttribute("attribute")) {
				return x.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
			}

			return false;
		});
	}

	internal static XdslElement? GetChildImpl(XdslElement element, string name)
	{
		return element.Find(x => {
			if (!x.HasAttribute("attribute") &&
				!x.HasAttribute("root")) {
				return x.Name == name;
			}

			return false;
		});
	}

	#endregion Validation

	#region Helpers

	internal static XdslSchemaValidationError MissingRootError(XdslDocument document, XdslDocument schema)
	{
		return new(XdslSchemaErrorType.MissingRoot, $"Root '{document.Name}' is not defined on Schema {schema.Name}.");
	}

	internal static XdslSchemaValidationError MissingAttribute(XdslElement attribute, XdslElement schemaImpl)
	{
		return new(XdslSchemaErrorType.MissingAttribute, $"Attribute {attribute.Name} on element {schemaImpl.Name} is required.");
	}

	internal static XdslSchemaValidationError MissingElement(XdslElement child, XdslElement schemaImpl)
	{
		return new(XdslSchemaErrorType.MissingElement, $"Element {child.Name} on element {schemaImpl.Name} is required.");
	}

	internal static XdslSchemaValidationError UnknownAttribute(XdslAttribute attribute, XdslElement schemaImpl)
	{
		return new(XdslSchemaErrorType.UnknowAttribute, $"Attribute '{attribute.Name}' is not defined on {schemaImpl.Name}.");
	}

	internal static XdslSchemaValidationError UnknownElement(XdslElement element, XdslElement schemaImpl)
	{
		return new(XdslSchemaErrorType.UnknowElement, $"Element '{element.Name}' is not defined on {schemaImpl.Name}.");
	}

	internal static XdslSchemaValidationError InvalidType(string? value, string name, string type, string xNode)
	{
		return new(XdslSchemaErrorType.InvalidType, $"Type '{value}' on {xNode} '{name}' is not a supported type. Schema type '{type}'.");
	}

	internal static XdslSchemaValidationError DoesNotSupportChildElements(XdslElement element)
	{
		return new(XdslSchemaErrorType.DoesNotSupportChildElements, $"Element '{element.Name}' cannot contain any child elements.");
	}

	#endregion Helpers
}