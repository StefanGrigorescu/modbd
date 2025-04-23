namespace MODBD_Common.Text;

/// <summary>
/// Large size TextBox. For inputs like Jsons.
/// </summary>
public sealed class TextBoxLg : Text
{
    public const int MaxLength = 850;

    public static TextBoxLg From(string? text)
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(text);
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, MaxLength);
        return new()
        {
            Value = text,
        };
    }

    private TextBoxLg() : base() { }
}
