using System;
using System.IO;
using System.Text;

namespace Realtin.Xdsl;

/// <summary>
/// Provides a class for XDSL elements to write their content to.
/// </summary>
public abstract partial class XdslTextWriter : IDisposable
{
	/// <summary>
	/// Closes this <see cref="XdslTextWriter"/>.
	/// <para>Note: The text writer returned by <see cref="Create(StringBuilder)"/>
	/// does not require to be closed.</para>
	/// </summary>
	public abstract void Close();

	/// <inheritdoc/>
	public void Dispose() => Close();

	/// <summary>
	/// Creates a new <see cref="XdslTextWriter"/> instance using the specified <see cref="StringBuilder"/>.
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static XdslTextWriter Create(StringBuilder builder) => new XdslTextWriter_TextBuilder(builder);

	/// <summary>
	/// Creates a new <see cref="XdslTextWriter"/> instance using the specified stream.
	/// </summary>
	/// <param name="stream"></param>
	/// <returns></returns>
	public static XdslTextWriter Create(Stream stream) => new XdslTextWriter_StreamWriter(new StreamWriter(stream));

	/// <summary>
	/// Creates a new <see cref="XdslTextWriter"/> instance using the specified filename.
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	public static XdslTextWriter Create(string filePath) => new XdslTextWriter_StreamWriter(new StreamWriter(filePath));
}

public abstract partial class XdslTextWriter
{
	internal int m_depth = 0;

	/// <summary>
	/// Initialize a new instance of the <see cref="XdslTextWriter"/> class.
	/// </summary>
	protected XdslTextWriter()
	{ }

	internal abstract void EnterChild();

	internal abstract void ExitChild();

	internal abstract void Indent();

	internal abstract void Write(char c);

	internal abstract void Write(ReadOnlySpan<char> chars);

	internal abstract void WriteLine();

	internal abstract void WriteLine(ReadOnlySpan<char> chars);
}