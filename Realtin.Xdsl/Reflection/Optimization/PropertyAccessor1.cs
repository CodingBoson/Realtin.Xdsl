using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Optimization;

internal sealed class PropertyAccessor<TTarget, TValue> : PropertyAccessor
{
	private Func<TTarget, TValue?> _getter = default!;

	private Action<TTarget, TValue?> _setter = default!;

	public override void Compile(PropertyInfo propertyInfo)
	{
		_getter = (Func<TTarget, TValue?>)propertyInfo.GetGetMethod(true).
			CreateDelegate(typeof(Func<TTarget, TValue?>));

		_setter = (Action<TTarget, TValue?>)propertyInfo.GetSetMethod(true).
			CreateDelegate(typeof(Action<TTarget, TValue?>));
	}

    public TValue? GetValue(TTarget target) => _getter(target);

    public void SetValue(TTarget target, TValue? value) => _setter(target, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object? GetValue(object instance) => GetValue((TTarget)instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SetValue(object instance, object? value)
		=> SetValue((TTarget)instance, (TValue?)value);
}