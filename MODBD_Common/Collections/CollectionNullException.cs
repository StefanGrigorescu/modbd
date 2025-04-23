using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MODBD_Common.Collections;

public sealed class CollectionNullException(string paramName) : Exception($"'{paramName}' must not be null!")
{
    /// <summary>
    /// This method checks validates the <paramref name="collection"/> parameter agains null values.
    /// </summary>
    /// <param name="collection">The text to be validated against null values.<br></br>
    /// If the method completes without throwing, this parameter is marked by compiler as not null.</param>
    /// <param name="valueParamName">
    /// The name of the <paramref name="collection"/> parameter. 
    /// It is an optional parameter and should be obtained by static analysis instead of manual passing.
    /// </param>
    /// <exception cref="CollectionNullException"></exception>
    public static void ThrowIfIsNull<T>(
        [NotNull] IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string valueParamName = "'collection'")
    {
        if (collection is null)
        {
            throw new CollectionNullException(valueParamName);
        }
    }
}
