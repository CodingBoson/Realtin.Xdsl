namespace Realtin.Xdsl.Schema;

public sealed class XdslIntegerTypeValidator : XdslTypeValidator
{
    public override bool Validate(string? type) => int.TryParse(type, out _);
}