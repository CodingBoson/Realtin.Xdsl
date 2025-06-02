namespace Realtin.Xdsl;

internal static class XdslAssert
{
	public static void ValidDocument(XdslDocument document)
	{
		if (document.Root is null) {
			throw new XdslException("XdslDocument does not have a root element.");
		}
	}
}