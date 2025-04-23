using System.Runtime.CompilerServices;

namespace MODBD_Common.Text;

interface ITextWithMaxLength
{
    abstract string? Value { get; init; }

    static abstract int MaxLength { get; }

    protected static void ValidateMaxLength<TTextWithMaxLength>(
        string? text,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'")
        where TTextWithMaxLength : ITextWithMaxLength
    {
        TextMaxLengthExceededException.ThrowIfIsMaxLengthExceeded(text, TTextWithMaxLength.MaxLength, textParamName);
    }
}
