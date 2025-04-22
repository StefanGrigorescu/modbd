using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MODBD_Api.Common.Collections;

public sealed class CollectionNullOrEmptyException : Exception
{
    /// <summary>
    /// This method checks validates the <paramref name="collection"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="collection">The text to be validated against null or empty values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="collection"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="CollectionNullOrEmptyException"></exception>
    public static void ThrowIfIsNullOrEmpty<T>(
        [NotNull] IReadOnlyList<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string valueParamName = "'collection'")
    {
        if (collection is null || !collection.Any())
        {
            throw new CollectionNullOrEmptyException(valueParamName);
        }
    }

    /// <summary>
    /// This method checks validates the <paramref name="collection"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="collection">The text to be validated against null or empty values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="collection"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="CollectionNullOrEmptyException"></exception>
    public static void ThrowIfIsNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string valueParamName = "'collection'")
    {
        if (collection is null || !collection.Any())
        {
            throw new CollectionNullOrEmptyException(valueParamName);
        }
    }

    private CollectionNullOrEmptyException(string paramName) : base(
        $"'{paramName}' must not be null or empty!"
    ) { }
}


public static class CollectionNullOrEmptyValidators
{
    /// <summary>
    /// This method checks validates the <paramref name="collection"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="collection">The text to be validated against null or empty values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="collection"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="CollectionNullOrEmptyException"></exception>
    public static IReadOnlyList<T> ValidateNotNullOrEmpty<T>(
        [NotNull] this IReadOnlyList<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string valueParamName = "'collection'")
    {
        CollectionNullOrEmptyException.ThrowIfIsNullOrEmpty(collection, valueParamName);
        return collection;
    }

    /// <summary>
    /// This method checks validates the <paramref name="collection"/> parameter agains null or emtpy values.
    /// </summary>
    /// <param name="collection">The text to be validated against null or empty values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="collection"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="CollectionNullOrEmptyException"></exception>
    public static IEnumerable<T> ValidateNotNullOrEmpty<T>(
        [NotNull] this IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string valueParamName = "'collection'")
    {
        CollectionNullOrEmptyException.ThrowIfIsNullOrEmpty(collection, valueParamName);
        return collection;
    }
}
