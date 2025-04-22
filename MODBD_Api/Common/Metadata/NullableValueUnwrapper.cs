using System.Linq.Expressions;

namespace MODBD_Api.Common.Metadata;

public sealed class NullableValueUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsGenericType &&
            propertyAccessor.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {        
            MemberExpression hasValueProperty = Expression.Property(propertyAccessor, "HasValue");

            BinaryExpression isValueNull = Expression.Equal(
                hasValueProperty, 
                Expression.Constant(false));

            // Access the Value property of Nullable<>
            MemberExpression valueProperty = Expression.Property(propertyAccessor, "Value");

            // Conditional expression to handle null value
            ConditionalExpression conditional = Expression.Condition(
                isValueNull,
                Expression.Constant(null, typeof(object)),
                IUnwrapper.UnwrapValueType<TEntity>(valueProperty, parameters).Body);

            return Expression.Lambda<Func<TEntity, object?>>(conditional, parameters);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
