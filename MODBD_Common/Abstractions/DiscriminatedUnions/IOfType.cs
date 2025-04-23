namespace MODBD_Common.Abstractions.DiscriminatedUnions;

public interface IOfType<TType> : IOfEitherType<TType>
    where TType : Enumeration<TType>
{
    static abstract TType ClassType { get; }
}
