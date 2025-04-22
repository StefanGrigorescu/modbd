namespace MODBD_Api.Common.Abstractions.DiscriminatedUnions;

public interface IOfEitherType<TType>
    where TType : Enumeration<TType>
{
    TType Type { get; }
}
