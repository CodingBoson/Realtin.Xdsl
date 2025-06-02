namespace Realtin.Xdsl.Schema;

public sealed class XdslFloatTypeValidator : XdslTypeValidator
{
    public override bool Validate(string? type) => float.TryParse(type, out _);
}