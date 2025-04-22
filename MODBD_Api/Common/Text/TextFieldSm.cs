namespace MODBD_Api.Common.Text;

/// <summary>
/// Small size TextField. For inputs like Names, Titles.
/// </summary>
public sealed class TextFieldSm : Text
{
    public const int MaxLength = 50;

    public static TextFieldSm From(string? text)
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(text);
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new()
        {
            Value = text,
        };
    }

    private TextFieldSm() : base() { }
}
