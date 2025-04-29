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
            if (! apps.IsSimplePredicateRelevant(
                new Conditions(completeMinimalSimplePredicates),
                simplePredicate,
                output
            )) {
                output.WriteLine($"Skipping {simplePredicate.Sql} as it is not revelant.\n");
                continue;
            }

            output.WriteLine($"Adding {simplePredicate.Sql} to complete minimal simple predicates.\n");
            completeMinimalSimplePredicates.Add(simplePredicate);

            output.WriteLine($"Checking complete minimal simple predicates set if any predicate is not relevant anymore.");
            int removedAnyPredicatesCount = 0;
            for(int i = 0; i < completeMinimalSimplePredicates.Count - 1; i++)
            {
                ICondition currentPredicate = completeMinimalSimplePredicates[i];
                if (currentPredicate == simplePredicate)
                {
                    continue;
                }

                if (! apps.IsSimplePredicateRelevant(
                    new Conditions(completeMinimalSimplePredicates.Where(p => p != currentPredicate)),
                    currentPredicate,
                    output
                )) {
                    output.WriteLine($"Removing {currentPredicate.Sql} from complete minimal simple predicates, as it is not revelant anymore.");
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

    private static bool IsSimplePredicateRelevant<TTable>(
        this EntityApplications<TTable> apps, 
        Conditions completeMinimalSimplePredicates, 
        ICondition simplePredicate,
        IOutput output
    )
        where TTable : Table<TTable>
    {
        output.WriteLine($"\nChecking relevance of {simplePredicate.Sql}");

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
