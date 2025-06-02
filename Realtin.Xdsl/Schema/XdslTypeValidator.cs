using System;
using System.Collections.Generic;

namespace Realtin.Xdsl.Schema;

/// <summary>
/// Provides a base class for XDSL type validators.
/// </summary>
public abstract class XdslTypeValidator
{
	private sealed class StringComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y) => x.Equals(y, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(string obj) => obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Type Validators to be used during the <see cref="XdslSchema"/> validation process.
	/// <para>Default Types:</para>
	/// <para>int/integer: <see cref="XdslIntegerTypeValidator"/></para>
	/// <para>float: <see cref="XdslFloatTypeValidator"/></para>
	/// <para>bool/boolean: <see cref="XdslBoolTypeValidator"/></para>
	/// <para>string: <see cref="XdslStringTypeValidator"/></para>
	/// <para>version: <see cref="XdslVersionTypeValidator"/></para>
	/// </summary>
	public static Dictionary<string, XdslTypeValidator> TypeValidators { get; }
		= new(new StringComparer()) {
			{ "int", new XdslIntegerTypeValidator() },
			{ "integer", new XdslIntegerTypeValidator() },
			{ "float", new XdslFloatTypeValidator() },
			{ "bool", new XdslBoolTypeValidator() },
			{ "boolean", new XdslBoolTypeValidator() },
			{ "string", new XdslStringTypeValidator() },
			{ "version", new XdslVersionTypeValidator() },
		};

	public abstract bool Validate(string? type);
}