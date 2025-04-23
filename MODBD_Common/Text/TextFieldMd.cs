namespace MODBD_Common.Text;

/// <summary>
/// Medium size TextField. For inputs like Short Descriptions.
/// </summary>
public sealed class TextFieldMd : Text
{
    public const int MaxLength = 150;

    public static TextFieldMd From(string? text)
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(text);
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new()
        {
            Value = text,
        };
    }

    private TextFieldMd() : base() { }
}
