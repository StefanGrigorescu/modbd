using MODBD_Api.Common.Abstractions.DiscriminatedUnions;
using MODBD_Api.Common.Text;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Api.Identity;

public class UnauthorizedResourceOperationException : Exception
{
    public required string Operation { get; init; }
    public required string ResourceName { get; init; }
    public required string? ResourceId { get; init; }
    public required string ReasonName { get; init; }

    private static string ResourceIdMessagePart(string? resourceId) =>
        resourceId is null ?
            string.Empty :
            $" #{resourceId}";

    [SetsRequiredMembers] protected UnauthorizedResourceOperationException(UnauthorizedResourceOperationExceptionCtorParams parameters) :
        base($"Operation '{parameters.Operation}' on the resource '{parameters.ResourceName}${ResourceIdMessagePart(parameters.ResourceId)}' is unauthorized! {parameters.Reason.Details}.") 
    {
        Operation = parameters.Operation;
        ResourceName = parameters.ResourceName;
        ResourceId = parameters.ResourceId;
        ReasonName = parameters.Reason;
    }
}

public sealed record UnauthorizedResourceOperationExceptionCtorParams
{
    public required UnauthorizedResourceOperation.OperationName Operation { get; init; }
    public required UnauthorizedResourceOperation.ResourceName ResourceName { get; init; }
    public required string? ResourceId { get; init; }
    public required UnauthorizedResourceOperation.Reason Reason { get; init; }
}


public static class UnauthorizedResourceOperation
{
    public sealed class OperationName : Text
    {
        public const int MaxLength = TextFieldSm.MaxLength;
        public static readonly OperationName Default = new() { Value = "Operation", };

        public static OperationName FromTrimUpper(string? text)
        {
            text = text?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(text))
            {
                return Default;
            }

            if (TextMaxLengthExceededException.IsMaxLengthExceeded(text, MaxLength))
            {
                return new() { Value = text[..MaxLength], };
            }

            return new() { Value = text, };
        }
        private OperationName() : base() { }
    }


    public sealed class ResourceName : Text
    {
        public const int MaxLength = TextFieldSm.MaxLength;
        public static readonly ResourceName Default = new() { Value = "Resource", };

        public static ResourceName FromTrimLower(string? text)
        {
            text = text?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(text))
            {
                return Default;
            }

            if (TextMaxLengthExceededException.IsMaxLengthExceeded(text, MaxLength))
            {
                return new() { Value = text[..MaxLength], };
            }

            return new() { Value = text, };
        }
        private ResourceName() : base() { }
    }


    public sealed class Reason : Enumeration<Reason>
    {
        public required string Details { get; init; }

        public static readonly Reason MissingRole = new()
        {
            Id = 0,
            Name = "Missing role",
            Details = "You do not have the required role",
        };
        public static readonly Reason MissingOwnership = new()
        {
            Id = 1,
            Name = "Missing ownership",
            Details = "You do not own the resource",
        };

        private Reason() { }
    }
}
