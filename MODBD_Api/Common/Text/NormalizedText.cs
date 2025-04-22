using System.Text.RegularExpressions;

namespace MODBD_Api.Common.Text;

public sealed class NormalizedText : Text
{
    public bool HasSubstring(string other) => 
        Value.Contains(
            ToNormalized(other));

    public static NormalizedText From(string text) => new()
    {
        Value = ToNormalized(text),
    };

    public static string ToNormalized(string text)
    {
        text = ReplaceNoiseSymbolsWithSpaces.Normalize(text);
        text = ReplaceHyphensWithSpaces.Normalize(text);                                         // Replace hyphens with spaces
        text = ReplaceMoreSubsequentSpacesWithSingleSpace.Normalize(text);         // Replace more subsequent whitespaces with a single space
        text = text.Trim();                                                                                             // Remove leading and trailing whitespaces
        text = ReplaceNonLatinCharactersWithLatinEquivalent.Normalize(text);         // Replace some special non-latin characters with latin equivalent.
        text = RemovePunctuation.Normalize(text);                                                      // Remove punctuation (after replacing non-latin characters)
        text = text.ToLower();                                                                                      // Convert to lowercase
        return text;
    }

    private NormalizedText() : base() {  }
}


public static partial class ReplaceHyphensWithSpaces
{
    public static string Normalize(string text) => 
        Hyphens().Replace(text, " ");

    [GeneratedRegex("-")]
    private static partial Regex Hyphens();
}


public static partial class ReplaceUnderscoresWithSpaces
{
    public static string Normalize(string text) =>
        Underscores().Replace(text, " ");

    [GeneratedRegex("_")]
    private static partial Regex Underscores();
}


public static partial class ReplaceDotsWithSpaces
{
    public static string Normalize(string text) =>
        Dots().Replace(text, " ");

    [GeneratedRegex(@"\.")]
    private static partial Regex Dots();
}


public static partial class ReplaceMoreSubsequentSpacesWithSingleSpace
{
    public static string Normalize(string text) => 
        MoreSubsequentSpaces().Replace(text, " ");

    [GeneratedRegex(@"\s+")]
    private static partial Regex MoreSubsequentSpaces();
}


public static partial class ReplaceNoiseSymbolsWithSpaces
{
    public static string Normalize(string text) => 
        NoiseSymbols().Replace(text, " ");

    [GeneratedRegex(@"â€”", RegexOptions.IgnoreCase)]
    private static partial Regex NoiseSymbols();
}

public static partial class ReplaceNonLatinCharactersWithLatinEquivalent
{
    public static string Normalize(string text)
    {
        text = NonLatinUppercaseCharacters().Replace(text, "U");
        text = NonLatinLowercaseCharacters().Replace(text, "u");
        return text;
    }

    [GeneratedRegex(@"Ãœ|Ü")]
    private static partial Regex NonLatinUppercaseCharacters();

    [GeneratedRegex(@"äü|ü")]
    private static partial Regex NonLatinLowercaseCharacters();
}


public static partial class RemovePunctuation
{
    public static string Normalize(string text) => 
        Punctuation().Replace(text, "");

    [GeneratedRegex(@"[^\w\s]|_")]
    private static partial Regex Punctuation();
}


public static partial class RemoveVowels
{
    public static string Normalize(string text) =>
        Vowels().Replace(text, "");

    [GeneratedRegex("[aeiouAEIOU]")]
    private static partial Regex Vowels();
}


public static class RemoveWhitespaces
{
    public static string Normalize(string text) => new (
        text
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray()
    );
}
