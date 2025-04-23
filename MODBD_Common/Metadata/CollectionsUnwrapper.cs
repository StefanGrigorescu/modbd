using System.Linq.Expressions;
using MODBD_Common.Collections;

namespace MODBD_Common.Metadata;

public sealed class CollectionsUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsACollectionType())
        {
            // More custom logic for collections to be defined later here
            // For now, we'll return null for these properties

            ConstantExpression nullValueProperty = Expression.Constant(
                null,
                typeof(object));

            return Expression.Lambda<Func<TEntity, object?>>(
                nullValueProperty,
                parameters);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
