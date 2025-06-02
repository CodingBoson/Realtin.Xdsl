namespace Realtin.Xdsl.Schema;

/// <summary>
/// Represents a schema validation error type.
/// </summary>
public enum XdslSchemaErrorType
{
	MissingRoot,
	MissingElement,
	MissingAttribute,
	InvalidType,
	UnknowElement,
	UnknowAttribute,
	DoesNotSupportChildElements
}