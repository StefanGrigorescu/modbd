using MODBD_Common.Collections;
using MODBD_Common.NumericTypes.Positive;
using MODBD_Common.Text;
using MODBD_Core.Applications.SqlConditions;
using MODBD_Core.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MODBD_Core.Applications;

public sealed class NoneApplication : IApplication
{
    public required TextFieldSm Name { get; init; } = TextFieldSm.From(nameof(NoneApplication));
    public required Positive<double> FrequencyPerMonth { get; init; } = Positive<double>.Zero;
    public required Positive<int> Selectivity { get; init; } = Positive<int>.Zero;
    public required string Sql { get; init; } = "Select 1 FROM Dual";
    public required IReadOnlyList<Column> AllColumns { get; init; } = CollectionsFactory.EmptyIReadOnlyList<Column>();
    public required Conditions WhereConditions { get; init; } = new([]);
    public required bool IsMain {  get; init; } = false;

    public static NoneApplication On(string table, Conditions whereConditions) => new()
    {
        WhereConditions = whereConditions,
        Sql = $"Select 1 FROM {table} \nWHERE " + string.Join("\n\tAND ", whereConditions.Select(c => c.Value.Sql)),
    };
    [SetsRequiredMembers] private NoneApplication() { }
}
