using System.Diagnostics;

namespace MODBD_Core.Applications;

[DebuggerDisplay("{Name}")]
public sealed record Operator
{
    public static readonly Operator LessThan = new() { Name = "<" };
    public static readonly Operator LessThanOrEqual = new() { Name = "<=" };
    public static readonly Operator GreaterThan = new() { Name = ">" };
    public static readonly Operator GreaterThanOrEqual = new() { Name = ">=" };
    public static readonly Operator Equal = new() { Name = "=" };
    public static readonly Operator NotEqual = new() { Name = "<>" };

    public static Operator Not(Operator op) => op switch
    {
        { Name: "<" } => GreaterThanOrEqual,
        { Name: "<=" } => GreaterThan,
        { Name: ">" } => LessThanOrEqual,
        { Name: ">=" } => LessThan,
        { Name: "=" } => NotEqual,
        { Name: "<>" } => Equal,
        _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
    };

    public required string Name { get; init; }

    private Operator() { }
}
