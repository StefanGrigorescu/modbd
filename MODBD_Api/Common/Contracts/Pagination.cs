using MODBD_Common.Abstractions;

namespace MODBD_Api.Common.Contracts;

public record PaginatedRequest
{
    public int? RowOffset { get; init; } = 0;
    public int? RowCount { get; init; } = 100;
}


public sealed class RowOffset : ValueObject<int>
{
    public static RowOffset FromNullableValue(int? value) =>
        value is null ?
            FromValue(0) :
            FromValue(value.Value);

    public static RowOffset FromValue(int value) =>
        value < 0 ?
            throw new ArgumentOutOfRangeException(nameof(value), "Row offset cannot be negative.") :
            new() { Value = value, };

    private RowOffset() { }
}


public sealed class RowCount : ValueObject<int>
{
    public static RowCount FromNullableValue(int? value) =>
        value is null ?
            FromValue(100) :
            FromValue(value.Value);

    public static RowCount FromValue(int value) =>
        value < 0 ?
            throw new ArgumentOutOfRangeException(nameof(value), "Row offset cannot be negative.") :
            new() { Value = value, };

    private RowCount() { }
}
