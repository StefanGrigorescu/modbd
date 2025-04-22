using MODBD_Api.Common.Collections;

namespace MODBD_Api.Common.Text;

/// <summary>
/// Nullable large size TextBox. For inputs like Jsons.
/// </summary>
public sealed class NullableTextBoxLg : NullableText
{
    public const int MaxLength = 850;

    public static readonly NullableTextBoxLg Null = new() { Value = null, };
    public static readonly NullableTextBoxLg Empty = new() { Value = string.Empty, };

    public static NullableTextBoxLg From(string? text)
    {
        if (text is null) { return Null; }
        if (text.IsNullOrEmpty()) { return Empty; }
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new() { Value = text, };
    }

    private NullableTextBoxLg() { }
}
