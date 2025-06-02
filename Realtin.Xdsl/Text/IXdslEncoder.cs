using System.Diagnostics.CodeAnalysis;

namespace Realtin.Xdsl.Text;

public interface IXdslEncoder
{
    string Encoding { get; }

    [return: NotNullIfNotNull(nameof(input))]
    string? Encode(string? input);

    [return: NotNullIfNotNull(nameof(input))]
    string? Decode(string? input);
}