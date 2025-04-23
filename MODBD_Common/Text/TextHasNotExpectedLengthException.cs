using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions.DomainExceptions;

namespace MODBD_Common.Text;

public sealed class TextHasNotExpectedLengthException : ValueObjectException
{
    /// <summary>
    /// Checks <paramref name="text"/>'s length 
    /// and throws a <see cref="TextHasNotExpectedLengthException"/> if it exceeds <paramref name="expectedLength"/>. 
    /// <para> If <paramref name="text"/> is null, it does not throw any error. </para>
    /// </summary>
    /// <param name="text">The text to have its length validated.</param>
    /// <param name="expectedLength">The expected length.</param>
    /// <param name="textParamName">
    /// The name of the <paramref name="text"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <param name="expectedLengthParamName">
    /// The name of the <paramref name="expectedLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="TextHasNotExpectedLengthException">If <paramref name="text"/>'s length is different than <paramref name="expectedLength"/>, it throws a text has not expected length exceeded exception.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expectedLength"/> is negative, it thows an argument out of range exception.</exception>
    public static void ThrowIfHasNotExpectedLength(
        string? text,
        int expectedLength,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'",
        [CallerArgumentExpression(nameof(expectedLength))] string expectedLengthParamName = "'expectedLength'")
    {
        if (HasNotExpectedLength(text, expectedLength, expectedLengthParamName))
        {
            throw new TextHasNotExpectedLengthException(text.Length, expectedLength, textParamName);
        }
    }


    /// <summary>
    /// Checks <paramref name="text"/>'s length 
    /// and returns true if it does <i>not</i> meet <paramref name="expectedLength"/> or false otherwise. 
    /// <para> If <paramref name="text"/> is null, it returns false. </para>
    /// <param name="text">The text to have its length checked.</param>
    /// <param name="expectedLength">The expected length of the text.</param>
    /// <param name="expectedLengthParamName">
    /// The name of the <paramref name="expectedLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>True if <paramref name="text"/> is null or its length is <paramref name="expectedLength"/> or false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expectedLength"/> is negative, it thows an argument out of range exception.</exception>
    public static bool HasNotExpectedLength(
        [NotNullWhen(true)] string? text,
        int expectedLength,
        [CallerArgumentExpression(nameof(expectedLength))] string expectedLengthParamName = "'expectedLength'")
    {
        if (expectedLength < 0)
        {
            throw new ArgumentOutOfRangeException(expectedLengthParamName);
        }

        if (text is null)
        {
            return false;
        }

        return text.Length > expectedLength;
    }


    private TextHasNotExpectedLengthException(int textLength, int expectedLength, string paramName) :
        base($"'{paramName}' value should be {expectedLength} characters. Current length is {textLength}.")
    { }
}
