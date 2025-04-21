using System.Diagnostics.CodeAnalysis;

namespace MODBD_Analiza.Applications;

internal sealed record SqlQueryParameter
{
    public required string Name { get; init; }

    public override string ToString() => "{" + Name + "}";

    [SetsRequiredMembers] public SqlQueryParameter(string name)
    {
        Name = name;
    }
}
