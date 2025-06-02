namespace Realtin.Xdsl.Schema;

public sealed class XdslBoolTypeValidator : XdslTypeValidator
{
    public override bool Validate(string? type) => bool.TryParse(type, out _);
}