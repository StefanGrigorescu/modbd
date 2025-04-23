using MODBD_Common.Collections;

namespace MODBD_Common.Text;

/// <summary>
/// Medium size TextField. For inputs like Short Descriptions.
/// </summary>
public sealed class NullableTextFieldMd : NullableText
{
    public const int MaxLength = 150;

    public static readonly NullableTextFieldMd Null = new() { Value = null, };
    public static readonly NullableTextFieldMd Empty = new() { Value = string.Empty, };

    public static NullableTextFieldMd From(string? text)
    {
        if (text is null) { return Null; }
        if (text.IsNullOrEmpty()) { return Empty; }
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new() { Value = text, };
    }

    private NullableTextFieldMd() { }
}
