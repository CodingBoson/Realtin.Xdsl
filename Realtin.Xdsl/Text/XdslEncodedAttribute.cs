using System;

namespace Realtin.Xdsl.Text;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class XdslEncodedAttribute : Attribute
{
    public string Encoding { get; set; } = "UTF-8";
}