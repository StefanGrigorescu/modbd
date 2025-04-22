using System.Linq.Expressions;
using MODBD_Api.Common.Abstractions.Entities;

namespace MODBD_Api.Common.Metadata;

public class IEntityIdUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsAssignableTo(typeof(EntityId<>)))
        {
            // Access the Value property of EntityIdBase<TValue>
            MemberExpression valueProperty = Expression.Property(propertyAccessor, "Value");

            return Expression.Lambda<Func<TEntity, object?>>(
                valueProperty,
                parameters);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
