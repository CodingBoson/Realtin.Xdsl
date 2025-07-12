using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Schema;

/// <summary>
/// Represents an XDSL Schema.
/// </summary>
[DebuggerDisplay("XdslSchema")]
public sealed partial class XdslSchema : IEquatable<XdslSchema?>
{
	[DebuggerDisplay("Schema")]
    internal readonly XdslDocument _schema;

    /// <summary>
    /// Initialize a new instance of the <see cref="XdslSchema"/> class.
    /// </summary>
    /// <param name="schema"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private XdslSchema(XdslDocument schema) 
		=> _schema = schema ?? throw new ArgumentNullException(nameof(schema));

    /// <summary>
    /// Assert <see cref="IsImplemented(XdslDocument)"/> returns true.
    /// </summary>
    /// <param name="document"></param>
    /// <exception cref="XdslSchemaException"></exception>
    public void AssertIsImplemented(XdslDocument document)
	{
		var result = Validate(document);
        
		if (result.HasErrors) {
			throw new XdslSchemaException(result.MakeErrorString());
		}
	}

    /// <summary>
    /// Checks if a document is implemented in this schema.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsImplemented(XdslDocument document) => Validate(document).Success;

    /// <summary>
    /// Validates a document and all it's children against this schema.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public XdslSchemaValidationResult Validate(XdslDocument document)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(document), document);

		XdslAssert.ValidDocument(document);

		var rootImpl = XdslSchemaImpl.GetRootImpl(_schema, document.Root!.Name);

		if (rootImpl == null) {
			return new(false, [XdslSchemaImpl.MissingRootError(document, _schema)]);
		}

		return XdslSchemaImpl.ValidateImpl(rootImpl, document.Root);
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as XdslSchema);

    /// <inheritdoc/>
    public bool Equals([NotNullWhen(true)] XdslSchema? other) 
		=> other is not null && _schema.Equals(other._schema);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_schema);

    /// <summary>
    /// Creates an <see cref="XdslSchema"/> from a file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static XdslSchema Create(string path) => new(XdslDocument.CreateFromFile(path));

    /// <summary>
    /// Creates an <see cref="XdslSchema"/> from an <see cref="XdslDocument"/>.
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static XdslSchema Create(XdslDocument schema) => new(schema);

    /// <summary>
    /// Creates an <see cref="XdslSchema"/> from a file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<XdslSchema> CreateAsync(string path) 
        => new XdslSchema(await XdslDocument.CreateFromFileAsync(path));

    /// <summary>
    /// Returns a value that indicates whether two <see cref="XdslSchema"/> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(XdslSchema? left, XdslSchema? right)
	{
		if (ReferenceEquals(left, right)) {
            return true;
        }

        return left is not null && left.Equals(right);
    }

    /// <summary>
    /// Returns a value that indicates whether two <see cref="XdslSchema"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    /// true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.
    /// </returns>
    public static bool operator !=(XdslSchema? left, XdslSchema? right) => !(left == right);
}