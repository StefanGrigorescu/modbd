using MODBD_Common.Abstractions;

namespace MODBD_Common.Text;

/// <summary>
/// Base class for text value objects.
/// </summary>
public abstract class Text : ValueObject<string>
{
    public override string ToString() =>
        Value;

    public bool NormalizedEquals(Text other) =>
        Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);

    protected Text() : base() { }
}
