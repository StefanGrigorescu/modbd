using MODBD_Analiza.Schema;

namespace MODBD_Analiza.Applications;

internal abstract record EntityApplications<TTable> 
    where TTable : Table<TTable>
{
    public required IReadOnlyList<IApplication> All { get; init; }
    public required IReadOnlyList<ICondition> AllSimplePredicates { get; init; }
}


internal static class EntityApplicationsFactories
{
    public static IReadOnlyList<IApplication> GetAllApplications<TTable>(this EntityApplications<TTable> apps)
        where TTable : Table<TTable>
        => apps
        .GetType()
        .GetProperties()
        .Where(p => p.PropertyType.IsAssignableTo(typeof(IApplication)))
        .ToIReadOnlyList(p => (IApplication)p.GetValue(apps)!);

    public static IReadOnlyList<ICondition> GetAllSimplePredicates(this IReadOnlyList<IApplication> allApplications) => allApplications
                .SelectMany(app => app.WhereConditions)
                .Distinct();
}
