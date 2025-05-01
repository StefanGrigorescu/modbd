using MODBD_Core.Applications.SqlConditions;
using MODBD_Core.IO;
using MODBD_Core.Schema;

namespace MODBD_Core.Applications;

public static class CompleteMinimalPredicates
{
    public static Conditions Of<TTable>(
        EntityApplications<TTable> apps,
        IOutput output
    )
        where TTable : Table<TTable>
    {
        List<ICondition> simplePredicates = [.. apps.AllSimplePredicates];
        List<ICondition> completeMinimalSimplePredicates = [];

        foreach (ICondition simplePredicate in simplePredicates)
        {
            if(completeMinimalSimplePredicates.Count == 0)  // Check relevance for the entire table
            {
                output.WriteLine($"\nChecking if {simplePredicate.Sql} is relevant for sharding in the {typeof(TTable).Name} table.");

                Conditions compositePredicate = new ([]);
                bool isRelevantForTable = apps.IsSimplePredicateRelevant(
                    simplePredicate,
                    compositePredicate,
                    output
                );
                if (!isRelevantForTable)
                {
                    output.WriteLine($"Predicate {simplePredicate.Sql} is not relevant for {typeof(TTable).Name} table.\n");
                    continue;
                }
                output.WriteLine($"Predicate {simplePredicate.Sql} is relevant for {typeof(TTable).Name} table.\n");
            }
            else    // Check relevance for any horizontal shard
            {
                output.WriteLine($"\nChecking if {simplePredicate.Sql} is relevant for sharding in any current horizontal shard.");

                IReadOnlyList<Conditions> compositePredicatesOverSimplePredicates = Conditions.CompositePredicatesFromSimplePredicates(completeMinimalSimplePredicates);
                output.WriteLine("Current composite predicates over the complete minimal simple predicates set are: {");
                for (int i = 0; i < compositePredicatesOverSimplePredicates.Count; i++)
                {
                    output.WriteLine($"\tm{i} =  {compositePredicatesOverSimplePredicates[i].ToSql()}");
                }
                output.WriteLine("}\n");

                bool isRelevantForAnyFragment = false;
                for (int i = 0; i < compositePredicatesOverSimplePredicates.Count; i++)
                {
                    Conditions compositePredicate = compositePredicatesOverSimplePredicates[i];
                    output.WriteLine($"Checking relevance of {simplePredicate.Sql} for m{i} =  {compositePredicate.ToSql()}");
                    bool isRelevantForCrtFragment = apps.IsSimplePredicateRelevant(
                        simplePredicate,
                        compositePredicate,
                        output
                    );
                    if (isRelevantForCrtFragment)
                    {
                        output.WriteLine($"Predicate {simplePredicate.Sql} is relevant for m{i}.\n");
                        isRelevantForAnyFragment = true;
                        break;
                    }
                    else
                    {
                        output.WriteLine($"Predicate {simplePredicate.Sql} is not relevant for m{i}.\n");
                    }
                }

                if (!isRelevantForAnyFragment)
                {
                    output.WriteLine($"Skipping {simplePredicate.Sql} as it is not revelant for any fragment.\n");
                    continue;
                }
            }

            output.WriteLine($"Adding {simplePredicate.Sql} to complete minimal simple predicates set.\n");
            completeMinimalSimplePredicates.Add(simplePredicate);

            output.WriteLine($"Checking complete minimal simple predicates set if any predicate is not relevant anymore.\n");
            int removedAnyPredicatesCount = 0;
            for (int i = 0; i < completeMinimalSimplePredicates.Count - 1; i++)
            {
                ICondition crtSimplePredicate = completeMinimalSimplePredicates[i];
                if (crtSimplePredicate == simplePredicate)
                {
                    continue;
                }

                output.WriteLine($"\nChecking if {crtSimplePredicate.Sql} is not relevant anymore.");
                bool isNotRelevantAnymore = apps.IsSimplePredicateNotRelevantAnymore(
                    crtSimplePredicate, 
                    completeMinimalSimplePredicates, 
                    output
                );
                if (isNotRelevantAnymore)
                {
                    output.WriteLine($"Removing {crtSimplePredicate.Sql} from complete minimal simple predicates set, as it is not revelant anymore.\n");
                    completeMinimalSimplePredicates.RemoveAt(i);
                    i--;
                    removedAnyPredicatesCount++;
                }
            }
            if( removedAnyPredicatesCount == 0)
            {
                output.WriteLine($"No predicates were removed from complete minimal simple predicates set.\n");
            }
            else
            {
                output.WriteLine($"{removedAnyPredicatesCount} irrelevant predicates were removed from complete minimal simple predicates set.\n");
            }
        }

        return new(completeMinimalSimplePredicates);
    }

    public static bool IsSimplePredicateNotRelevantAnymore<TTable>(
        this EntityApplications<TTable> apps,
        ICondition crtSimplePredicate,
        IReadOnlyList<ICondition> completeMinimalSimplePredicates,
        IOutput output
    )
        where TTable : Table<TTable>
    {
        IReadOnlyList<Conditions> compositePredicatesOverSimplePredicates = Conditions.CompositePredicatesFromSimplePredicates(
            completeMinimalSimplePredicates
                .Where((condition) => !condition.Equals(crtSimplePredicate))     // Exclude the current simple predicate, as it will be checked against the resulting composite predicate
                .ToArray()
        );

        for (int j = 0; j < compositePredicatesOverSimplePredicates.Count; j++)
        {
            Conditions compositePredicate = compositePredicatesOverSimplePredicates[j];
            output.WriteLine($"Checking relevance of {crtSimplePredicate.Sql} for m'{j} =  {compositePredicate.ToSql()}");
            
            bool isRelevantForCrtFragment = apps.IsSimplePredicateRelevant(
                crtSimplePredicate,
                compositePredicate,
                output
            );
            if (isRelevantForCrtFragment)
            {
                output.WriteLine($"Predicate {crtSimplePredicate.Sql} is relevant for m'{j}.\n");
                return false;   // false = negation of "is not relevant anymore"
            }            
            output.WriteLine($"Predicate {crtSimplePredicate.Sql} is not relevant for m'{j}.\n");
        }

        return true;    // true = is not relevant anymore
    }

    private static bool IsSimplePredicateRelevant<TTable>(
        this EntityApplications<TTable> apps, 
        ICondition simplePredicate,
        Conditions completeMinimalSimplePredicates, 
        IOutput output
    )
        where TTable : Table<TTable>
    {
        IApplication firstFragment = apps.FindByWhereConditions(
            completeMinimalSimplePredicates.And(simplePredicate),
            output
        );
        IApplication secondFragment = apps.FindByWhereConditions(
            completeMinimalSimplePredicates.And(simplePredicate.Not()),
            output
        );

        double firstFragmentRatio = firstFragment.FrequencyPerMonth / (firstFragment.Selectivity + 1);
        double secondFragmentRatio = secondFragment.FrequencyPerMonth / (secondFragment.Selectivity + 1);

        string comparisonSingn = firstFragmentRatio != secondFragmentRatio ? "<>" : "=";
        output.WriteLine($"Comparing  {firstFragment.FrequencyPerMonth} / ({firstFragment.Selectivity} + 1)  and  {secondFragment.FrequencyPerMonth} / ({secondFragment.Selectivity} + 1): ");
        output.WriteLine($"\t{firstFragmentRatio} {comparisonSingn} {secondFragmentRatio}");

        return firstFragmentRatio != secondFragmentRatio;
    }
}
