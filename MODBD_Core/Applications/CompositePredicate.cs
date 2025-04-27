using System.Numerics;

namespace MODBD_Core.Applications;

public static class CompositePredicate
{
    public static bool HasNoSense(this Conditions compositePredicate)
    {
        foreach (ICondition condition in compositePredicate.Values)
        {
            foreach (ICondition otherCondition in compositePredicate.Values)
            {
                if (
                    !condition.Equals(otherCondition) && (
                        AreMutuallyExclusive<int>(condition, otherCondition) ||
                        AreMutuallyExclusive<double>(condition, otherCondition) ||
                        AreMutuallyExclusive<long>(condition, otherCondition)
                    )
                )
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool AreMutuallyExclusive<TSqlValue>(ICondition condition, ICondition otherCondition)
        where TSqlValue : struct, IComparable<TSqlValue>, IEquatable<TSqlValue>, INumber<TSqlValue>, IMinMaxValue<TSqlValue> =>
        condition is ConditionWithValue<TSqlValue> conditionWithValue &&
        otherCondition is ConditionWithValue<TSqlValue> otherConditionWithValue &&
        conditionWithValue.Column.Equals(otherConditionWithValue.Column) &&
        !conditionWithValue.Codomain.Intersect(otherConditionWithValue.Codomain).Any();
}
