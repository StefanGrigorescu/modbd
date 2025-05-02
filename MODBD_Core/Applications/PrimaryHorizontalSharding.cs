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
        string tableName = typeof(TTable).Name;

        IReadOnlyList<ICondition> completeMinimalPredicates = CompleteMinimalPredicates.Of(apps, output);
        output.Write($"Complete minimal predicates of {tableName}: ");
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

        compositePredicates = RemoveCompositePredicatesWithNoSense(compositePredicates, output);

        output.Write($"Composite predicates of {tableName}: ");
        output.WriteLine("{");
        for (int i = 0; i < compositePredicates.Count; i++)
        {
            output.WriteLine($"\tm{i} =  {compositePredicates[i].ToSql()}");
        }
        output.WriteLine("}\n");

        output.WriteLine("Simplifying composite predicates.\n");
        compositePredicates = SimplifyCompositePredicates(compositePredicates, output);
        output.WriteLine("Simplified composite predicates: {");
        for (int i = 0; i < compositePredicates.Count; i++)
        {
            output.WriteLine($"\tm{i} =  {compositePredicates[i].ToSql()}");
        }
        output.WriteLine("}\n");

        return compositePredicates;
    }

    private static IReadOnlyList<Conditions> RemoveCompositePredicatesWithNoSense(
        IReadOnlyList<Conditions> compositePredicates,
        IOutput output
    ) {
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

        if (removedIndexes.Count == 0)
        {
            output.WriteLine("No composite predicate had no sense.\n");
            return compositePredicates;
        }

        output.WriteLine("Removing composite predicates with no sense.\n");
        return compositePredicates
            .Where((Conditions _, int idx) =>
                !removedIndexes.Contains(idx)
            ).ToIReadOnlyList();
    }

    private static IReadOnlyList<Conditions> SimplifyCompositePredicates(
        IReadOnlyList<Conditions> compositePredicates,
        IOutput output
    ) {
        (Conditions SimplifiedConditions, bool HadChanged)[] simplifyResponse = compositePredicates
            .Select((Conditions compositePredicate) =>
            {
                (Conditions SimplifiedConditions, bool HadChanged) simplifiedCompositePredicate = compositePredicate.Simplify();
                if (simplifiedCompositePredicate.HadChanged)
                {
                    output.WriteLine($"Composite predicate  {compositePredicate.ToSql()}  is equivalent to the simplified predicate  {simplifiedCompositePredicate.SimplifiedConditions.ToSql()} .");
                }
                return simplifiedCompositePredicate;
            }).ToArray();

        bool wasAnySimplified = simplifyResponse.Any(t => t.HadChanged);
        if ( ! wasAnySimplified)
        {
            output.WriteLine("All composite predicate were already as simple as possible.\n");
            return simplifyResponse.ToIReadOnlyList(t => t.SimplifiedConditions);
        }

        // Removing duplicates from simplified composite predicates, if any
        List<Conditions> simplifiedCompositePredicates = [];
        foreach ((Conditions simplifiedCompositePredicate, bool hadChanged) in simplifyResponse)
        {
            if (!simplifiedCompositePredicates.Contains(simplifiedCompositePredicate))
            {
                simplifiedCompositePredicates.Add(simplifiedCompositePredicate);
            }
        }
        if (simplifyResponse.Length != simplifiedCompositePredicates.Count)
        {
            output.WriteLine("Removing duplicates from simplified composite predicates.\n");
            return simplifiedCompositePredicates.ToIReadOnlyList();
        }

        output.WriteLine();
        return simplifiedCompositePredicates.ToIReadOnlyList();
    }
}
