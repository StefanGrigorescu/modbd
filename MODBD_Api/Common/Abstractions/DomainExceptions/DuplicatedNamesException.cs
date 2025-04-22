using MODBD_Api.Common.Collections;
using MODBD_Api.Common.Text;

namespace MODBD_Api.Common.Abstractions.DomainExceptions;

public class DuplicatedNamesException : ValueObjectException
{
    public static void ThrowIfHasDuplicatedNames<TKey, TValue, TText>(
        Dictionary<TKey, TValue> collection,
        string collectionTypeName
    )
        where TKey : notnull
        where TValue : IWithName<TText>
         where TText : Text.Text
    {
        if (collection
                .Select(kvp => kvp.Value.Name)
                .HasDuplicatedItems())
        {
            throw new DuplicatedNamesException(collectionTypeName, typeof(TValue).Name);
        }
    }

    public static void ThrowIfHasDuplicatedNames<TValue, TText>(
        IReadOnlyList<TValue> collection,
        string collectionTypeName
    )
        where TValue : IWithName<TText>
         where TText : Text.Text
    {
        if (collection
                .Select(kvp => kvp.Name)
                .HasDuplicatedItems())
        {
            throw new DuplicatedNamesException(collectionTypeName, typeof(TValue).Name);
        }
    }

    private DuplicatedNamesException(string collectionTypeName, string itemsTypeName) : base(
        $"{collectionTypeName} must not have {itemsTypeName} items with duplicated names!.")
    { }
}
