using System.Collections.ObjectModel;
using System.Text.Json;
using MODBD_Common.Abstractions;

namespace MODBD_Common.Collections;

public class MetadataCollection :
    ReadOnlyDictionary<string, object>,
    IReadOnlyDictionary<string, object>,
    IEquatable<MetadataCollection>,
    IImmutable<MetadataCollection>,
    IJson
{
    public bool Equals(MetadataCollection? other) =>
        this.IReadOnlyDictionaryEqual(other);

    public override bool Equals(object? obj) =>
        obj is not null &&
        obj is MetadataCollection other &&
        ((IEquatable<MetadataCollection>)this).Equals(other);

    public override int GetHashCode() =>
        this.IReadOnlyDictionaryHashCode();

    public static readonly new MetadataCollection Empty = new() { };
    /// <summary>
    /// This represents the max length of the string representation of a <see cref="MetadataCollection"/>.
    /// </summary>
    public const int SerializedMaxLength = 850;

    /// <summary>
    /// This method should be userd for database records deserialization only, 
    /// because it does not perform additional validations (database records are in consistent state).
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static MetadataCollection Deserialize(string? source)
    {
        if (source.IsNullOrEmpty())
        {
            return Empty;
        }

        Dictionary<string, object> deserializedDictionary =
            JsonSerializer.Deserialize<Dictionary<string, object>>(source)!;

        return new(deserializedDictionary);
    }

    public string Serialize() =>
        JsonSerializer.Serialize(this);

    public static MetadataCollection FromDictionary(Dictionary<string, object> source) => source.IsNullOrEmpty() ? Empty : new(source) { };
    public static MetadataCollection WithSingleItem(string key, object value) => new(new Dictionary<string, object>()
    {
        { key, value },
    });

    /// <summary>
    /// This constructor initializes a <see cref="MetadataCollection"/> containing the data from the <paramref name="source"/>.<br></br>
    /// </summary>
    protected MetadataCollection(Dictionary<string, object> source) :
        base(source)
    { }

    /// <summary>
    /// This constructor initializes an empty <see cref="MetadataCollection"/>. <br></br>
    /// It is also to be used by the ORM. 
    /// </summary>
    private MetadataCollection() :
        base(CollectionsFactory.EmptyReadOnlyDictionary<string, object>())
    { }
}
