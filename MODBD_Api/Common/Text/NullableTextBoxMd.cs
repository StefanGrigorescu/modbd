using MODBD_Api.Common.Collections;

namespace MODBD_Api.Common.Text;

/// <summary>
/// Medium size TextBox. For inputs like Descriptions.
/// </summary>
public sealed class NullableTextBoxMd : NullableText
{
    public const int MaxLength = 550;

    public static readonly NullableTextBoxMd Null = new() { Value = null, };
    public static readonly NullableTextBoxMd Empty = new() { Value = string.Empty, };

    public static NullableTextBoxMd From(string? text)
    {
        if (text is null) { return Null; }
        if (text.IsNullOrEmpty()) { return Empty; }
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new() { Value = text, };
    }

    private NullableTextBoxMd() { }
}
