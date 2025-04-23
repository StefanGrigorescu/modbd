using MODBD_Common.Abstractions.DiscriminatedUnions;

namespace MODBD_Common.Abstractions.Responses;

public sealed class FailureReason : Enumeration<FailureReason>
{
    public static readonly FailureReason BadRequest = new() { Id = 0, Name = "BadRequest", };
    public static readonly FailureReason Validation = new() { Id = 1, Name = "Validation", };
    public static readonly FailureReason Unauthorized = new() { Id = 2, Name = "Unauthorized", };
    public static readonly FailureReason LockedOut = new() { Id = 3, Name = "LockedOut", };

    private FailureReason() { }
}
