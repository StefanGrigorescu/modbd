namespace MODBD_Common.Abstractions.Events;

public abstract class AppEventId : ValueObject<DateTime>
{
    public override string ToString() =>
        Value.ToString();

    protected AppEventId() : base() { }
}
