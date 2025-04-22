namespace MODBD_Api.Common.Abstractions.Events;

public abstract class AppEvent<TId>
    where TId : AppEventId
{
    public required TId CreatedOn { get; init; }
}
