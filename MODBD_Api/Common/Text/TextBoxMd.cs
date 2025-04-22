namespace MODBD_Api.Common.Text;

/// <summary>
/// Medium size TextBox. For inputs like Descriptions.
/// </summary>
public sealed class TextBoxMd : Text
{
    public const int MaxLength = 550;

    public static TextBoxMd From(string? text)
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(text);
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new()
        {
            Value = text,
        };
    }

    private TextBoxMd() : base() { }
}
