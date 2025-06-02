using System;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;
using static Realtin.Xdsl.Xql.XqlCondition;

namespace Realtin.Xdsl.Xql.Compilers;

internal static class XqlConditionCompiler
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlCondition Compile(ReadOnlySpan<char> conditionExpression)
	{
		int num = conditionExpression.IndexOf(' ');
		var propertyExpression = conditionExpression[..num].Trim();
		conditionExpression = conditionExpression[num..].TrimStart();

		num = conditionExpression.IndexOf(' ');
		var operatorSpan = conditionExpression[..num].Trim();
		conditionExpression = conditionExpression[num..].TrimStart();

		num = conditionExpression.IndexOf('"');
		var valueSpan = conditionExpression.Slice(num + 1, conditionExpression.LastIndexOf('"') - 1);

		var @operator = XqlParser.ParseOperator(operatorSpan);
		var value = valueSpan.ToString();

		return Compile(propertyExpression, value, @operator);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static XqlCondition Compile(ReadOnlySpan<char> propertyExpression, string value, XqlOperator @operator)
	{
		var splitter = new StringSplitter(propertyExpression);

		var propertyExpressions = new CompiledPropertyExpressions();
		while (splitter.TrySplit(':', out var segment, trimEntries: true)) {
			if (segment.Equals("X", StringComparison.OrdinalIgnoreCase)) {
				continue;
			}
			else if (segment.Equals("NAME", StringComparison.OrdinalIgnoreCase)) {
				propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Name, null));
			}
			else if (segment.Equals("TYPE", StringComparison.OrdinalIgnoreCase)) {
				propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Type, null));
			}
			else if (segment.Equals("TEXT", StringComparison.OrdinalIgnoreCase)) {
				propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Text, null));
			}
			else if (segment.StartsWith("ATTRIBUTE", StringComparison.OrdinalIgnoreCase)) {
				int num = segment.IndexOf('[');

				if (num < 0) {
					propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Attribute, null));

					continue;
				}

				int num2 = segment.IndexOf(']');
				var text = segment.Slice(num + 1, num2 - num - 1);

				propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Attribute, text.ToString()));
			}
			else if (segment.StartsWith("CHILD", StringComparison.OrdinalIgnoreCase)) {
				int num = segment.IndexOf('[');

				if (num < 0) {
					propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Child, null));

					continue;
				}

				int num2 = segment.IndexOf(']');
				var textSpan = segment.Slice(num + 1, num2 - num - 1);

				propertyExpressions.Add(new CompiledPropertyExpression(XqlProperty.Child, textSpan.ToString()));
			}
			else {
				throw new XqlException($"Property {segment.ToString()} is not a recognized Property.");
			}
		}

		return new XqlCondition(propertyExpressions, @operator, value);
	}
}