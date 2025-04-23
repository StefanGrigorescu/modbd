using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MODBD_Common.NullableTypes;

public sealed class NullValueException(string paramName) : Exception($"'{paramName}' must not be null!")
{
    /// <summary>
    /// This method checks validates the <paramref name="value"/> parameter agains null values.
    /// </summary>
    /// <param name="value">The text to be validated against null values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="value"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="NullValueException"></exception>
    public static void ThrowIfIsNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string valueParamName = "'value'")
        where T : struct
    {
        if (!value.HasValue)
        {
            throw new NullValueException(valueParamName);
        }
    }

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
    /// <exception cref="NullValueException"></exception>
    public static void ThrowIfIsNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string paramName = "'value'")
        where T : class
    {
        if (value is null)
        {
            throw new NullValueException(paramName);
        }
    }
}


public static class NullableTypesNotNullValidators
{
    /// <summary>
    /// This method  validates the <paramref name="value"/> parameter agains null values.
    /// </summary>
    /// <typeparam name="T">The parameter generic type. It is a value type.</typeparam>
    /// <param name="value">
    /// The text to be validated against null values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.
    /// </param>
    /// <param name="paramName">
    /// The name of the <paramref name="value"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <returns>
    /// If <paramref name="value"/> is not null, this method returns its wrapped <typeparamref name="T"/> instance. <br>
    /// Otherwise, it throws a <see cref="NullValueException"/>.
    /// </returns>
    /// <exception cref="NullValueException"></exception>
    public static T ValidateNotNull<T>(
        [NotNull] this T? value,
        [CallerArgumentExpression(nameof(value))] string paramName = "'parameter'") 
        where T: struct
    {
        NullValueException.ThrowIfIsNull(value, paramName);

        return value.Value;
    }

    /// <summary>
    /// This method  validates the <paramref name="value"/> parameter agains null values.
    /// </summary>
    /// <typeparam name="T">The parameter generic type. It is a reference type.</typeparam>
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
    public static T ValidateNotNull<T>(
        [NotNull] this T? value,
        [CallerArgumentExpression(nameof(value))] string paramName = "'parameter'") 
        where T : class
    {
        NullValueException.ThrowIfIsNull(value, paramName);

        return value;
    }
}
