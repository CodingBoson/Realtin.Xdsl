using System;

namespace Realtin.Xdsl.Serialization;

public abstract partial class XdslSerializer
{
    public abstract string GetXName(Type type);

    public abstract bool CanSerialize(Type type);

    public abstract void Serialize(XdslWriter writer, object? value, XdslSerializerOptions options);

    public abstract object? Deserialize(XdslReader reader, Type type, XdslSerializerOptions options);
}