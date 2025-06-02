namespace Realtin.Xdsl.Serialization;

public enum SelfReferenceHandling : byte
{
	Ignore = 0,
	Throw = 1,
	Serialize = 2,
}