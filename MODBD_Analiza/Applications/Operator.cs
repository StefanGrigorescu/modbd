namespace MODBD_Analiza.Applications;

internal sealed record Operator
{
    public static Operator LessThan = new() { Name = "<" };
    public static Operator LessThanOrEqual = new() { Name = "<=" };
    public static Operator GreaterThan = new() { Name = ">" };
    public static Operator GreaterThanOrEqual = new() { Name = ">=" };
    public static Operator Equal = new() { Name = "=" };
    public static Operator NotEqual = new() { Name = "<>" };

    public required string Name { get; init; }

    private Operator() { }
}
