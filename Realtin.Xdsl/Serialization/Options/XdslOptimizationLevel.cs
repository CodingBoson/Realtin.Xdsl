namespace Realtin.Xdsl.Serialization;

public enum XdslOptimizationLevel : byte
{
	/// <summary>
	/// No optimization for fields and properties.
	/// </summary>
	None = 0,

	/// <summary>
	/// Only Properties get optimized.
	/// </summary>
	Low = 1,

	/// <summary>
	/// Both Properties and Fields get optimized.
	/// <para>Fields are optimized using <see cref="System.Reflection.Emit.DynamicMethod"/> and <see cref="System.Reflection.Emit.ILGenerator"/></para>
	/// </summary>
	High = 2,
}