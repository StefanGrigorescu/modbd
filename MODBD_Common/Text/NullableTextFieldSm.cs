using MODBD_Common.Collections;

namespace MODBD_Common.Text;

/// <summary>
/// Small size TextField. For inputs like Names, Titles.
/// </summary>
public sealed class NullableTextFieldSm : NullableText
{
    public const int MaxLength = 50;

    public static readonly NullableTextFieldSm Null = new() { Value = null, };
    public static readonly NullableTextFieldSm Empty = new() { Value = string.Empty, };

    public static NullableTextFieldSm From(string? text)
    {
        if (text is null) { return Null; }
        if (text.IsNullOrEmpty()) { return Empty; }
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new() { Value = text, };
    }

    private NullableTextFieldSm() { }
}
