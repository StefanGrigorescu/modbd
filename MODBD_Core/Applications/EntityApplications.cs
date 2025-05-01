using MODBD_Core.Schema;
using MODBD_Common.Collections;
using MODBD_Core.IO;
using MODBD_Core.Applications.SqlConditions;
using MODBD_Common.Abstractions.Responses;

namespace MODBD_Core.Applications;

public abstract record EntityApplications<TTable> 
    where TTable : Table<TTable>
{
    public required IReadOnlyList<IApplication> All { get; init; }
    public required IReadOnlyList<IApplication> AllMain { get; init; }
    public required IReadOnlyList<ICondition> AllSimplePredicates { get; init; }

    public AppResponse<IApplication> FindByWhereConditions(
        Conditions conditions,
        IOutput output
    ) {
        bool hasNoSense = CompositePredicate.HasNoSense(conditions);
        if(hasNoSense)
        {
            string message = $"Composite predicate  {conditions.ToSql()}  has no sense. No application could use it.";
            output.WriteLine(message);
            return AppResponse<IApplication>.Failed(message);
        }

        (Conditions simplifiedConditions, bool hadChanged) = conditions.Simplify();
        if(hadChanged)
        {
            output.WriteLine($"Composite predicate  {conditions.ToSql()}  is equivalent to the simplified predicate  {simplifiedConditions.ToSql()}.");
        }

        IApplication? app = All.SingleOrDefault(app =>
            app.WhereConditions.Equals(simplifiedConditions)
        );

        if(app is null)
        {
            output.WriteLine($"No application uses the composite predicate  {conditions.ToSql()} . Using 'none' application.");
            return AppResponse<IApplication>.Succeeded(NoneApplication.On(typeof(TTable).Name, conditions));
        }

        return AppResponse<IApplication>.Succeeded(app);
    }
}


public static class EntityApplicationsFactories
{
    public static IReadOnlyList<IApplication> GetAllApplications<TTable>(this EntityApplications<TTable> apps)
        where TTable : Table<TTable>
        => apps
        .GetType()
        .GetProperties()
        .Where(p => p.PropertyType.IsAssignableTo(typeof(IApplication)))
        .ToIReadOnlyList(p => (IApplication)p.GetValue(apps)!);

    public static IReadOnlyList<IApplication> GetAllMainApplications(this IReadOnlyList<IApplication> all)
        => all
            .Where(app => app.IsMain)
            .ToIReadOnlyList();

    public static IReadOnlyList<ICondition> GetAllSimplePredicates(this IReadOnlyList<IApplication> allApplications) => allApplications
        .Where(app => app.IsMain)
        .SelectMany(app => app
            .WhereConditions
            .Select(kvp => kvp.Value)
        ).Distinct()
        .ToIReadOnlyList();
}
