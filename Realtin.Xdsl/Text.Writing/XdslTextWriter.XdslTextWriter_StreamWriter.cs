using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl;

public abstract partial class XdslTextWriter
{
	private sealed class XdslTextWriter_StreamWriter(StreamWriter textWriter) : XdslTextWriter
	{
		private readonly StreamWriter _streamWriter = textWriter;

		public override void Close() => _streamWriter.Close();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void EnterChild()
		{
			m_depth++;
			WriteLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void ExitChild() => m_depth--;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Indent()
		{
			for (int i = 0; i < m_depth; i++) {
				_streamWriter.Write('\t');
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Write(char c) => _streamWriter.Write(c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void Write(ReadOnlySpan<char> chars) => _streamWriter.Write(chars);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void WriteLine()
		{
			_streamWriter.WriteLine();
			Indent();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override void WriteLine(ReadOnlySpan<char> chars)
		{
			_streamWriter.Write(chars);
			_streamWriter.WriteLine();
			Indent();
		}
	}
}