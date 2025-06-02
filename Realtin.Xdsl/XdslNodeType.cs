namespace Realtin.Xdsl
{
    /// <summary>
    /// The type of a node.
    /// </summary>
    public enum XdslNodeType : byte
    {
        Document = 0,
        Tag = 1,
        Comment = 2,
        Element = 3,
    }
}