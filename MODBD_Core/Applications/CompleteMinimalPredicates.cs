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
                simplePredicate
            )) {
                continue;
            }

            output.WriteLine($"Adding {simplePredicate.Sql} to complete minimal predicates.");
            completeMinimalSimplePredicates.Add(simplePredicate);

            for(int i = 0; i < completeMinimalSimplePredicates.Count - 1; i++)
            {
                ICondition currentPredicate = completeMinimalSimplePredicates[i];
                if (currentPredicate == simplePredicate)
                {
                    continue;
                }

                if (! apps.IsSimplePredicateRelevant(
                    new Conditions(completeMinimalSimplePredicates.Where(p => p != currentPredicate)),
                    currentPredicate
                )) {
                    output.WriteLine($"Removing {currentPredicate.Sql} from complete minimal predicates.");
                    completeMinimalSimplePredicates.RemoveAt(i);
                    i--;
                }
            }
        }

        return new(completeMinimalSimplePredicates);
    }

    private static bool IsSimplePredicateRelevant<TTable>(
        this EntityApplications<TTable> apps, 
        Conditions completeMinimalSimplePredicates, 
        ICondition simplePredicate
    )
        where TTable : Table<TTable>
    {
        IApplication firstFragment = apps.SingleByWhereConditions(
            completeMinimalSimplePredicates.And(simplePredicate)
        );
        IApplication secondFragment = apps.SingleByWhereConditions(
            completeMinimalSimplePredicates.And(simplePredicate.Not())
        );

        double firstFragmentRatio = firstFragment.FrequencyPerMonth / firstFragment.Selectivity;
        double secondFragmentRatio = secondFragment.FrequencyPerMonth / secondFragment.Selectivity;

        return firstFragmentRatio != secondFragmentRatio;
    }
}
