using System.Text;

namespace MODBD_Api.Common.Text;

public static class RemoveDuplicateEmojis
{
    public static string Normalize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        StringBuilder result = new();
        MaybeEmoji previous = MaybeEmoji.None;

        for (int i = 0; i < text.Length; i++)
        {
            MaybeEmoji current = MaybeEmoji.FromTextAtIndex(text, i);

            if (
                current.Character != previous.Character ||
                !current.IsCharacterEmoji ||
                !previous.IsCharacterEmoji
            )
            {
                result.Append(current);
                previous = current;
            }

            if (char.IsSurrogatePair(text, i))
            {
                i++; // Skip the next character as it's part of a surrogate pair
            }
        }

        return result.ToString();
    }
}

public sealed record MaybeEmoji
{
    public required string Character { get; init; }
    public required bool IsCharacterEmoji { get; init; }

    public override string ToString() => Character;

    public static readonly MaybeEmoji None = new() { Character = "", IsCharacterEmoji = false };

    public static MaybeEmoji FromTextAtIndex(string text, int index)
    {
        string character = char.ConvertFromUtf32(char.ConvertToUtf32(text, index));
        return new()
        {
            Character = character,
            IsCharacterEmoji = IsEmoji(character),
        };
    }

    private static bool IsEmoji(string c)
    {
        int codePoint = char.ConvertToUtf32(c, 0);
        return (codePoint >= 0x1F600 && codePoint <= 0x1F64F) || // Emoticons
               (codePoint >= 0x1F300 && codePoint <= 0x1F5FF) || // Miscellaneous Symbols and Pictographs
               (codePoint >= 0x1F680 && codePoint <= 0x1F6FF) || // Transport and Map Symbols
               (codePoint >= 0x1F700 && codePoint <= 0x1F77F) || // Alchemical Symbols
               (codePoint >= 0x1F900 && codePoint <= 0x1F9FF) || // Supplemental Symbols and Pictographs
               (codePoint >= 0x1FA70 && codePoint <= 0x1FAFF);   // Symbols and Pictographs Extended-A
    }

    private MaybeEmoji() { }
}
