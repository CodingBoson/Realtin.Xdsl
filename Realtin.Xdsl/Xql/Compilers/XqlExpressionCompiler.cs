using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Xql.Compilers;

internal static class XqlExpressionCompiler
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlExpression Compile(ReadOnlySpan<char> expression)
	{
		try {
			return CompileImpl(ref expression);
		}
		catch (Exception ex) when (ex is not XqlException) {
			throw new XqlException($"Invalid Expression '{expression.ToString()}'.", ex);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XqlExpression CompileImpl(ref ReadOnlySpan<char> expression)
	{
		int num = expression.IndexOf(' ');
		var querySpan = expression[..num].Trim();
		expression = expression[num..].Trim();

		num = expression.LastIndexOf(' ');
		var methodSpan = expression[num..].Trim();
		expression = expression[..num].Trim();

		var splitter = new StringSplitter(expression);

		List<XqlCondition> conditions = [];
		while (splitter.TrySplit('&', out var conditionExpression, trimEntries: true)) {
			var compiledCondition = XqlConditionCompiler.Compile(conditionExpression);

			conditions.Add(compiledCondition);
		}

		return new XqlExpression(XqlParser.ParseQuery(querySpan), XqlParser.ParseMethod(methodSpan), conditions);
	}
}