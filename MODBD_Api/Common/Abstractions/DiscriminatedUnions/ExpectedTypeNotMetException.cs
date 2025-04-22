using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Abstractions.DiscriminatedUnions;

public sealed class ExpectedTypeNotMetException<TType> : ValueObjectException
    where TType : Enumeration<TType>
{
    private readonly string _expectedType;
    private readonly string _actualType;

    public static ExpectedType WithExpectedType(TType expectedType)
        => new(expectedType.Name);

    public static ExpectedType WithExpectedType(string expectedType) 
        => new(expectedType);

    public void ThrowIfIsExpectedTypeNotMet()
    {
        if (_expectedType != _actualType)
        {
            throw this;
        }
    }

    private ExpectedTypeNotMetException(string expectedType, string actualType) :
        base($"Expected value of type {expectedType}! Value's actual type is {actualType}.")
    {
        _expectedType = expectedType;
        _actualType = actualType;
    }


    public sealed class ExpectedType
    {
        private readonly string _expectedType;

        public ExpectedType(string expectedTypeName)
        {
            _expectedType = expectedTypeName;
        }

        public ExpectedTypeNotMetException<TType> WithActualType(TType actualType) =>
            new(_expectedType, actualType.Name);

        public ExpectedTypeNotMetException<TType> WithActualType(string actualType) => 
            new(_expectedType, actualType);
    }
}
