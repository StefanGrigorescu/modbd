using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Text;

public sealed class TextMinLengthNotMetException : ValueObjectException
{
    /// <summary>
    /// This method checks <paramref name="text"/>'s length 
    /// and throws a <see cref="TextMinLengthNotMetException"/> if it does not meet <paramref name="minLength"/>. 
    /// <para> If <paramref name="text"/> is null, it does not throw any error. </para>
    /// </summary>
    /// <param name="text">The text to have its length validated.</param>
    /// <param name="minLength">The minimum length allowed for the text.</param>
    /// <param name="textParamName">
    /// The name of the <paramref name="text"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <param name="minLengthParamName">
    /// The name of the <paramref name="minLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="TextMinLengthNotMetException">If <paramref name="text"/>'s length does not meet <paramref name="minLength"/>, it throws a text min length not met exception.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="minLength"/> is negative, it thows an argument out of range exception.</exception>
    public static void ThrowIfIsMinLengthNotMet(
        string? text,
        int minLength,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'",
        [CallerArgumentExpression(nameof(minLength))] string minLengthParamName = "'minLength'")
    {
        if (IsMinLengthNotMet(text, minLength, minLengthParamName))
        {
            throw new TextMinLengthNotMetException(text.Length, minLength, textParamName);
        }
    }


    /// <summary>
    /// This method checks <paramref name="text"/>'s length 
    /// and returns true if it does not meet <paramref name="minLength"/> or false otherwise. 
    /// <para> If <paramref name="text"/> is null, it returns false. </para>
    /// <param name="text">The text to have its length checked.</param>
    /// <param name="minLength">The minimum length allowed for the text.</param>
    /// <param name="minLengthParamName">
    /// The name of the <paramref name="minLength"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>True if <paramref name="text"/> is null or its length does not meet <paramref name="minLength"/> or false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="minLength"/> is negative, it thows an argument out of range exception.</exception>
    private static bool IsMinLengthNotMet(
        [NotNullWhen(true)] string? text,
        int minLength,
        [CallerArgumentExpression(nameof(minLength))] string minLengthParamName = "'minLength'")
    {
        if (minLength < 0)
        {
            throw new ArgumentOutOfRangeException(minLengthParamName);
        }

        if (text is null)
        {
            return false;
        }

        return text.Length < minLength;
    }


    private TextMinLengthNotMetException(int textLength, int minLength, string paramName) :
        base($"'{paramName}' value is too short! It should have at least {minLength} characters. Current length is {textLength}.")
    { }
}
