using System.Linq.Expressions;
using MODBD_Api.Common.Abstractions;

namespace MODBD_Api.Common.Metadata;

public sealed class ValueObjectUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsAssignableTo(typeof(ValueObject<>)))
        {
            // Access the Value property of IText
            MemberExpression valueProperty = Expression.Property(propertyAccessor, "Value");

            return Expression.Lambda<Func<TEntity, object?>>(
                valueProperty,
                parameters);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
