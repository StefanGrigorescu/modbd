using MODBD_Common.Collections;
using MODBD_Core.Applications.SqlConditions;
using MODBD_Core.IO;
using MODBD_Core.Schema;

namespace MODBD_Core.Applications;

public static class PrimaryHorizontalSharding
{
    public static IReadOnlyList<Conditions> Of<TTable>(
        EntityApplications<TTable> apps,
        IOutput output
    )
        where TTable : Table<TTable>
    {
        IReadOnlyList<ICondition> completeMinimalPredicates = CompleteMinimalPredicates.Of(apps, output);
        output.Write($"Complete minimal predicates of {typeof(TTable).Name}: ");
        output.WriteLine("{");
        foreach (ICondition condition in completeMinimalPredicates)
        {
            output.WriteLine($"\t{condition.Sql}");
        }
        output.WriteLine("}\n");

        IReadOnlyList<Conditions> compositePredicates = Conditions.CompositePredicatesFromSimplePredicates(completeMinimalPredicates);
        output.Write($"All composite predicates over complete minimal predicates set: ");
        output.WriteLine("{");
        foreach (Conditions compositePredicate in compositePredicates)
        {
            output.WriteLine($"\t{compositePredicate.ToSql()}");
        }
        output.WriteLine("}\n");

        output.WriteLine("Checking if any composite predicate has no sense.\n");

        HashSet<int> removedIndexes = [];
        for (int i = 0; i < compositePredicates.Count; i++)
        {
            Conditions compositePredicate = compositePredicates[i];

            bool hasNoSense = CompositePredicate.HasNoSense(compositePredicate);
            if (hasNoSense)
            {
                output.WriteLine($"Composite predicate {compositePredicate.ToSql()} has no sense.");
                removedIndexes.Add(i);
            }
        }

        if(removedIndexes.Count == 0)
        {
            output.WriteLine("No composite predicate had no sense.\n");
        } 
        else
        {
            output.WriteLine("Removing composite predicates with no sense.\n");
        }

        return compositePredicates
            .Where((Conditions _, int idx) => 
                ! removedIndexes.Contains(idx)
            ).ToIReadOnlyList();
    }
}
