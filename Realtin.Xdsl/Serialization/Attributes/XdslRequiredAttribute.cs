namespace Realtin.Xdsl.Serialization;

/// <summary>
/// Indicates that the annotated member must bind to an XDSL property on deserialization.
/// </summary>
public sealed class XdslRequiredAttribute : XdslSerializerAttribute
{
	public XdslRequiredAttribute()
	{
	}
}