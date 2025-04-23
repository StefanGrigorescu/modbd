using MODBD_Common.Abstractions;

namespace MODBD_Common.Text;

/// <summary>
/// Base class for nullable text value objects.
/// </summary>
public abstract class NullableText : NullableValueObject<string>
{
    protected NullableText() { }
}
