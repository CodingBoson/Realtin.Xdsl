namespace Realtin.Xdsl.Xql;

internal interface IXqlCondition
{
	bool IsConditionMet(XdslElement element, XqlVariables variables);
}