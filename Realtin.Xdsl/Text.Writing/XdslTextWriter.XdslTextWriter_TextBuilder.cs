using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Realtin.Xdsl;

public abstract partial class XdslTextWriter
{
	private sealed class XdslTextWriter_TextBuilder(StringBuilder textWriter) : XdslTextWriter
	{
		private readonly StringBuilder _stringBuilder = textWriter;

		public override void Close()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void EnterChild()
		{
			m_depth++;
			WriteLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void ExitChild() => m_depth--;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Indent() => _stringBuilder.Append('\t', m_depth);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Write(char c) => _stringBuilder.Append(c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Write(ReadOnlySpan<char> chars) => _stringBuilder.Append(chars);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void WriteLine()
		{
			_stringBuilder.AppendLine();
			Indent();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void WriteLine(ReadOnlySpan<char> chars)
		{
			_stringBuilder.Append(chars);
			_stringBuilder.AppendLine();
			Indent();
		}
	}
}