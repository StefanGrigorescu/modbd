using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions.DomainExceptions;

namespace MODBD_Common.Text;

public sealed class TextMaxLengthExceededException : ValueObjectException
{
    /// <summary>
    /// This method checks <paramref name="text"/>'s length 
    /// and throws a <see cref="TextMaxLengthExceededException"/> if it exceeds <paramref name="maxLength"/>. 
    /// <para> If <paramref name="text"/> is null, it does not throw any error. </para>
    /// </summary>
    /// <param name="text">The text to have its length validated.</param>
    /// <param name="maxLength">The maximum length allowed for the text.</param>
    /// <param name="textParamName">
    /// The name of the <paramref name="text"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <param name="maxLengthParamName">
    /// The name of the <paramref name="maxLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="TextMaxLengthExceededException">If <paramref name="text"/>'s length exceeds <paramref name="maxLength"/>, it throws a text max length exceeded exception.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxLength"/> is negative, it thows an argument out of range exception.</exception>
    public static void ThrowIfIsMaxLengthExceeded(
        string? text,
        int maxLength,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'",
        [CallerArgumentExpression(nameof(maxLength))] string maxLengthParamName = "'maxLength'")
    {
        if (IsMaxLengthExceeded(text, maxLength, maxLengthParamName))
        {
            throw new TextMaxLengthExceededException(text.Length, maxLength, textParamName);
        }
    }


    /// <summary>
    /// This method checks <paramref name="text"/>'s length 
    /// and returns true if it exceeds <paramref name="maxLength"/> or false otherwise. 
    /// <para> If <paramref name="text"/> is null, it returns false. </para>
    /// <param name="text">The text to have its length checked.</param>
    /// <param name="maxLength">The maximum length allowed for the text.</param>
    /// <param name="maxLengthParamName">
    /// The name of the <paramref name="maxLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>True if <paramref name="text"/> is null or its length does not exceed <paramref name="maxLength"/> or false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxLength"/> is negative, it thows an argument out of range exception.</exception>
    public static bool IsMaxLengthExceeded(
        [NotNullWhen(true)] string? text,
        int maxLength,
        [CallerArgumentExpression(nameof(maxLength))] string maxLengthParamName = "'maxLength'")
    {
        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(maxLengthParamName);
        }

        if (text is null)
        {
            return false;
        }

        return text.Length > maxLength;
    }


    private TextMaxLengthExceededException(int textLength, int maxLength, string paramName) :
        base($"'{paramName}' value is too long! It should have at most {maxLength} characters. Current length is {textLength}.")
    { }
}
