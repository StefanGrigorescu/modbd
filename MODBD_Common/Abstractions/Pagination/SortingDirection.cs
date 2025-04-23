using MODBD_Common.Abstractions.DiscriminatedUnions;

namespace MODBD_Common.Abstractions.Pagination;

public sealed class SortingDirection : Enumeration<SortingDirection>
{
    public static readonly SortingDirection Ascending = new() { Id = 0, Name = "Ascending", };
    public static readonly SortingDirection Descending = new() { Id = 1, Name = "Descending", };

    public static SortingDirection Default =>
        Descending;

    private SortingDirection() { }
}


public enum SortingDirectionRequest
{
    Ascending = 0,
    Descending = 1,
}
