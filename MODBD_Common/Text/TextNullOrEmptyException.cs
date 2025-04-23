using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions.DomainExceptions;
using MODBD_Common.NullableTypes;

namespace MODBD_Common.Text;

public sealed class TextNullOrEmptyException: ValueObjectException
{
    /// <summary>
    /// This method checks validates the <paramref name="text"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="text">The text to be validated against null or empty values.  <br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="textParamName">
    /// The name of the <paramref name="text"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="TextNullOrEmptyException"></exception>
    public static void ThrowIfIsNullOrEmpty(
        [NotNull] string? text,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'")
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new TextNullOrEmptyException(textParamName);
        }
    }

    /// <summary>
    /// This method checks validates the <paramref name="text"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="text">The text to be validated against null or empty values.  <br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="textParamName">
    /// The name of the <paramref name="text"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="TextNullOrEmptyException"></exception>
    public static void ThrowIfIsNullOrEmptyWithCustomExceptionMessage(
        [NotNull] string? text,
        string customExceptionMessage,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'")
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new TextNullOrEmptyException(textParamName, customExceptionMessage);
        }
    }

    private TextNullOrEmptyException(string paramName) :
        base($"'{paramName}' must not be null or empty!") { }

    private TextNullOrEmptyException(string paramName, string customExceptionMessage) :
        base($"'{paramName}' must not be null or empty! {customExceptionMessage}") { }
}


public static class StringNotNullOrEmptyValidators
{
    /// <summary>
    /// This method  validates the <paramref name="value"/> parameter agains null values.
    /// </summary>
    /// <param name="value">
    /// The text to be validated against null values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.
    /// </param>
    /// <param name="paramName">
    /// The name of the <paramref name="value"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>
    /// If <paramref name="value"/> is not null, this method returns it. <br>
    /// Otherwise, it throws a <see cref="NullValueException"/>.
    /// </returns>
    /// <exception cref="NullValueException"></exception>
    public static string ValidateNotNull(
        [NotNull] this string? value,
        [CallerArgumentExpression(nameof(value))] string paramName = "'parameter'")
    {
        NullValueException.ThrowIfIsNull(value, paramName);

        return value;
    }

    /// <summary>
    /// This method  validates the <paramref name="value"/> parameter against null or empty values.
    /// </summary>
    /// <param name="value">
    /// The text to be validated.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.
    /// </param>
    /// <param name="paramName">
    /// The name of the <paramref name="value"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>
    /// If <paramref name="value"/> is not null, this method returns it. <br>
    /// Otherwise, it throws a <see cref="NullValueException"/>.
    /// </returns>
    /// <exception cref="NullValueException"></exception>
    public static string ValidateNotNullOrEmpty(
        [NotNull] this string? value,
        [CallerArgumentExpression(nameof(value))] string paramName = "'parameter'")
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(value, paramName);

        return value;
    }
}
