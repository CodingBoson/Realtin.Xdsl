using System.Collections.Generic;

namespace Realtin.Xdsl.Xql
{
	internal sealed class XqlQuery
	{
		public List<XqlExpression> Expressions { get; }

		public XqlQuery(List<XqlExpression> expressions)
		{
			Expressions = expressions;
		}

		public static XqlQuery Create(string xqlQuery)
		{
			var expressionsAsText = xqlQuery.Split(',');

			var expressions = new List<XqlExpression>();

			foreach (var expressionAsText in expressionsAsText) {
				expressions.Add(XqlExpression.Compile(expressionAsText));
			}

			return new XqlQuery(expressions);
		}
	}
}