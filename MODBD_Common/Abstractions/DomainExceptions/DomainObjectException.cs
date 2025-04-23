using System.Runtime.CompilerServices;
using MODBD_Common.Collections;

namespace MODBD_Common.Abstractions.DomainExceptions;

public class DomainObjectException : Exception
{
    public string? ThrownBy { get; set; }
    public MetadataCollection ErrorMetadata { get; init; } = MetadataCollection.Empty;

    public virtual void ThrowIf(bool condition, [CallerMemberName] string caller = "caller")
    {
        if (condition)
        {
            ThrownBy = caller;
            throw this;
        }
    }

    public override string ToString() =>
        $"{GetType().Name}: {Message}";

    public DomainObjectException(string message) : base(message) { }
}
