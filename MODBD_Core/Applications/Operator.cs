using MODBD_Common.Abstractions.DiscriminatedUnions;
using System.Diagnostics;

namespace MODBD_Core.Applications;

[DebuggerDisplay("{Name}")]
public sealed class Operator : Enumeration<Operator>
{
    public static readonly Operator LessThan = new() { Id = 0, Name = "<", };
    public static readonly Operator LessThanOrEqual = new() {Id = 1, Name = "<=", };
    public static readonly Operator GreaterThan = new() { Id = 2, Name = ">", };
    public static readonly Operator GreaterThanOrEqual = new() { Id = 3, Name = ">=", };
    public static readonly Operator Equal = new() { Id = 4, Name = "=", };
    public static readonly Operator NotEqual = new() { Id = 5, Name = "<>", };

    public static Operator Not(Operator op) => op.Match<Operator>(new()
    {
        GreaterThanOrEqual = _ => LessThan,
        GreaterThan = _ => LessThanOrEqual,
        LessThanOrEqual = _ => GreaterThan,
        LessThan = _ => GreaterThanOrEqual,
        Equal = _ => NotEqual,
        NotEqual = _ => Equal
    });

    public TResponse Match<TResponse>(MatchOperator<TResponse> match) => this switch
    {
        { Name: "<" } => match.LessThan(this),
        { Name: "<=" } => match.LessThanOrEqual(this),
        { Name: ">" } => match.GreaterThan(this),
        { Name: ">=" } => match.GreaterThanOrEqual(this),
        { Name: "=" } => match.Equal(this),
        { Name: "<>" } => match.NotEqual(this),
        _ => throw TypeUndefinedException<Operator>.WithName(Name)
    };

    private Operator() { }
}


public sealed record MatchOperator<TResponse>
{
    public required Func<Operator, TResponse> GreaterThanOrEqual { get; init; }
    public required Func<Operator, TResponse> GreaterThan { get; init; }
    public required Func<Operator, TResponse> LessThanOrEqual { get; init; }
    public required Func<Operator, TResponse> LessThan { get; init; }
    public required Func<Operator, TResponse> Equal { get; init; }
    public required Func<Operator, TResponse> NotEqual { get; init; }
}
