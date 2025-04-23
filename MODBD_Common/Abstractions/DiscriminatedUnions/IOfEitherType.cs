namespace MODBD_Common.Abstractions.DiscriminatedUnions;

public interface IOfEitherType<TType>
    where TType : Enumeration<TType>
{
    TType Type { get; }
}
