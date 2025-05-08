using System.Diagnostics;
using System.Reflection;
using MODBD_Common.Collections;
using MODBD_Common.NullableTypes;
using MODBD_Common.Text;

namespace MODBD_Common.Abstractions.DiscriminatedUnions;

[DebuggerDisplay("{typeof(TEnum).Name} #{Id} {Name}")]
public abstract class Enumeration<TEnum> : 
    IEquatable<Enumeration<TEnum>>, 
    IComparable<Enumeration<TEnum>>,
    IImmutable<Enumeration<TEnum>>,
    IImmutable<TEnum>
    where TEnum : Enumeration<TEnum>
{
    public required int Id { get; init; }
    public required string Name { get; init; }

    public static TEnum FromId(int id) =>
        All.TryGetValue(id, out TEnum? enumeration) ?
            enumeration :
            throw TypeUndefinedException<TEnum>.WithId(id);

    public static TEnum FromName(string name)
    {
        TextNullOrEmptyException.ThrowIfIsNullOrEmpty(name);

        return All
            .Values
            .FirstOrDefault(enumeration => enumeration.Name.Equals(name)) ??
            throw TypeUndefinedException<TEnum>.WithName(name);
    }

    public static string AllCsv => string.Join(
        ", ", 
        All.Select(enumeration => enumeration.Value.Name));

    public static readonly IReadOnlyDictionary<int, TEnum> All = CreateAll();

    public override string ToString() =>
        Name;

    public static implicit operator string(Enumeration<TEnum> enumeration) => 
        enumeration.Name;

    bool IEquatable<Enumeration<TEnum>>.Equals(Enumeration<TEnum>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        Id == other.Id;

    public bool Equals(TEnum? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        Id == other.Id;

    public bool Equals(string? other) =>
        Name.Equals(other);

    public bool Equals(int other) =>
        Id.Equals(other);
    
    public override bool Equals(object? obj) =>
        obj is Enumeration<TEnum> other &&
        ((IEquatable<Enumeration<TEnum>>)this).Equals(other);

    public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        (left is null && right is null) || (
            left is not null &&
            right is not null &&
            left.Equals(right)
        );

    public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        !(left == right);

    public override int GetHashCode() =>
        Id.GetHashCode();
    
    private static IReadOnlyDictionary<int, TEnum> CreateAll()
    {
        Type enumType = typeof(TEnum);

        IReadOnlyDictionary<int, TEnum> all = enumType
            .GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy
            ).Where((FieldInfo fieldInfo) => fieldInfo.FieldType.IsAssignableTo(enumType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!)
            .ToIReadOnlyDictionary(enumeration => enumeration.Id);  // If there are duplicate Ids, an exception will be thrown when creating the dictionary

        IReadOnlyDictionary<string, int> duplicatedNames = DuplicatedNamesOf(all);
        if (duplicatedNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"The following names are duplicated in {enumType.Name}: {string.Join(", ", duplicatedNames.Select(kvp => $"{kvp.Key} x{kvp.Value}"))}.");
        }

        return all;
    }

    private static IReadOnlyDictionary<string, int> DuplicatedNamesOf(IReadOnlyDictionary<int, TEnum> all)
    {
        Dictionary<string, int> namesAndCounts = [];
        foreach (TEnum enumeration in all.Values)
        {
            if (namesAndCounts.TryGetValue(enumeration.Name, out int count))
            {
                namesAndCounts[enumeration.Name] = count + 1;
            }
            else
            {
                namesAndCounts[enumeration.Name] = 1;
            }
        }
        return namesAndCounts
            .Where(kvp => kvp.Value > 1)
            .ToIReadOnlyDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public int CompareTo(Enumeration<TEnum>? other) =>
        other is not null &&
        GetType() == other.GetType() ?
            Id.CompareTo(other.Id) :
            CompareToResult.WhenOtherIsNull;

    protected Enumeration() { }
}
