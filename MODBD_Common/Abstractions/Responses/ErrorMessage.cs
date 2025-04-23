using MODBD_Common.Collections;

namespace MODBD_Common.Abstractions.Responses;

public sealed record ErrorMessage(string Value)
{
    public static implicit operator ErrorMessage(string source) =>
        new(source);

    public override string ToString() =>
        Value;

    public static readonly IReadOnlyList<ErrorMessage> EmptyIReadOnlyList = CollectionsFactory.EmptyIReadOnlyList<ErrorMessage>();
}
