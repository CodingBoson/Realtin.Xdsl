using System;
using System.Diagnostics;

namespace Realtin.Xdsl;

/// <summary>
/// Represents an XDSL document type.
/// </summary>
[DebuggerDisplay("DocumentType, {KnownType}")]
public readonly struct XdslDocumentType
{
	/// <summary>
	/// Represents a document type enumeration.
	/// </summary>
	public enum DocType
	{
		/// <summary>
		/// An XDSL document.
		/// </summary>
		Document,

		/// <summary>
		/// An XDSL schema.
		/// </summary>
		Schema,

		/// <summary>
		/// An unknown document type.
		/// </summary>
		Unknown
	}

	/// <summary>
	/// The XDSL document type. (Read Only)
	/// </summary>
	public static readonly XdslDocumentType Document = new("Document", DocType.Document);

	/// <summary>
	/// The XDSL schema type. (Read Only)
	/// </summary>
	public static readonly XdslDocumentType Schema = new("Schema", DocType.Schema);

	/// <summary>
	/// The <see cref="string"/> document type.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// The <see cref="DocType"/> document type.
	/// </summary>
	public DocType KnownType { get; }

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslDocumentType"/> structure.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="knownType"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public XdslDocumentType(string type, DocType knownType)
	{
		Type = type ?? throw new ArgumentNullException(nameof(type));
		KnownType = knownType;
	}

	/// <summary>
	/// Creates an <see cref="XdslDocumentType"/> structure
	/// from a <see cref="string"/> representing a document type.
	/// </summary>
	/// <param name="docType"></param>
	/// <returns></returns>
	public static XdslDocumentType Create(string docType)
	{
		if (docType.Equals("document", StringComparison.OrdinalIgnoreCase)) {
			return new XdslDocumentType(docType, DocType.Document);
		}
		else if (docType.Equals("schema", StringComparison.OrdinalIgnoreCase)) {
			return new XdslDocumentType(docType, DocType.Schema);
		}

		return new XdslDocumentType(docType, DocType.Unknown);
	}
}