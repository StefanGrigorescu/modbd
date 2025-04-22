using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Abstractions.DiscriminatedUnions;

public sealed class TypeUndefinedException<TType> : ValueObjectException
    where TType : Enumeration<TType>
{
    public static TypeUndefinedException<TType> WithName(string typeName) => new(
        $"{typeof(TType).Name} '{typeName}' is undefined! Required one of available types: {Enumeration<TType>.AllCsv}.");

    public static TypeUndefinedException<TType> WithId(int typeId) => new(
        $"{typeof(TType).Name} with id {typeId} is undefined! Required one of available types: {Enumeration<TType>.AllCsv}.");

    public static TypeUndefinedException<TType> FromCustomMessage(string customMessage) => new(customMessage);

    private TypeUndefinedException(string message) : base(message) { }
}


public sealed class TypeUndefinedException : ValueObjectException
{
    public static TypeUndefinedException FromCustomMessage(string customMessage) => new(customMessage);

    private TypeUndefinedException(string message) : base(message) { }
}
