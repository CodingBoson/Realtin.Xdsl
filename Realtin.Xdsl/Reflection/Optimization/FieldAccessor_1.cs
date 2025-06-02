using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Optimization;

internal sealed class FieldAccessor<TTarget, TValue> : FieldAccessor
{
	private Func<TTarget, TValue?> _getter = default!;

	private Action<TTarget, TValue?> _setter = default!;

	public override void Compile(FieldInfo fieldInfo)
	{
		_getter = CreateGetter(fieldInfo);
		_setter = CreateSetter(fieldInfo);
	}

    public TValue? GetValue(TTarget target) => _getter(target);

    public void SetValue(TTarget target, TValue? value) => _setter(target, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object? GetValue(object instance) => GetValue((TTarget)instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SetValue(object instance, object? value) => SetValue((TTarget)instance, (TValue?)value);

    private static Func<TTarget, TValue?> CreateGetter(FieldInfo field)
	{
		string methodName = field.ReflectedType.FullName + ".get_" + field.Name;

		DynamicMethod getterMethod = new(methodName, field.FieldType, [field.DeclaringType], true);

		ILGenerator gen = getterMethod.GetILGenerator();

		gen.Emit(OpCodes.Ldarg_0);
		gen.Emit(OpCodes.Ldfld, field);

		gen.Emit(OpCodes.Ret);

		var getterType = typeof(Func<TTarget, TValue?>);

		return (Func<TTarget, TValue?>)getterMethod.CreateDelegate(getterType);
	}

	private static Action<TTarget, TValue?> CreateSetter(FieldInfo field)
	{
		string methodName = field.ReflectedType.FullName + ".set_" + field.Name;

		DynamicMethod setterMethod = new(methodName, null, [field.DeclaringType, field.FieldType], true);

		ILGenerator gen = setterMethod.GetILGenerator();

		gen.Emit(OpCodes.Ldarg_0);
		gen.Emit(OpCodes.Ldarg_1);
		gen.Emit(OpCodes.Stfld, field);

		gen.Emit(OpCodes.Ret);

		var setterType = typeof(Action<TTarget, TValue?>);

		return (Action<TTarget, TValue?>)setterMethod.CreateDelegate(setterType);
	}
}