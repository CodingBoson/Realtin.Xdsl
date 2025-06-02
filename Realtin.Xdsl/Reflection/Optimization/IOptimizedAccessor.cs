namespace Realtin.Xdsl.Optimization;

internal interface IOptimizedAccessor
{
	object? GetValue(object target);

	void SetValue(object target, object? value);
}