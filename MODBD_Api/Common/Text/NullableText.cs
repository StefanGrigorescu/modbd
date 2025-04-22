using MODBD_Api.Common.Abstractions;

namespace MODBD_Api.Common.Text;

/// <summary>
/// Base class for nullable text value objects.
/// </summary>
public abstract class NullableText : NullableValueObject<string>
{
    protected NullableText() { }
}
