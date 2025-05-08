using System.Runtime.CompilerServices;
using MODBD_Common.Collections;

namespace MODBD_Common.Abstractions.DomainExceptions;

public class ValueObjectException : Exception
{
    public string? ThrownBy { get; set; }
    public MetadataCollection ErrorMetadata { get; init; } = MetadataCollection.Empty;

    public virtual void ThrowIf(bool condition, [CallerMemberName] string caller = "caller")
    {
        if (condition)
        {
            ThrownBy = caller;
            throw this;
        }
    }

    public static T TryReturnResultOrThrowIfCaughtException<T>(
        Func<T> funcThatMightThrowValueObjectException,
        Func<ValueObjectException, string> outerExceptionMessageFactory, 
        [CallerMemberName] string caller = "caller"
    ) {
        try
        {
            return funcThatMightThrowValueObjectException();
        }
        catch (ValueObjectException innerException)
        {
            // rethrow exception with additional information
            throw new ValueObjectException(
                outerExceptionMessageFactory(innerException),
                innerException);
        }
    }

    public override string ToString() =>
        $"{GetType().Name}: {Message}";

    public ValueObjectException(string message) : base(message) { }

    public ValueObjectException(string message, ValueObjectException innerException) : base(message, innerException) { }
}
