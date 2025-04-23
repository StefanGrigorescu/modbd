using System.Linq.Expressions;
using MODBD_Common.Abstractions;

namespace MODBD_Common.Metadata;

public sealed class NullableValueObjectUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    private readonly NullableValueUnwrapper _nullableValueUnwrapper = new(DefaultUnwrapper.Instance);

    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsAssignableTo(typeof(NullableValueObject<>)))
        {
            // Access the Value property of INullableText
            MemberExpression valueProperty = Expression.Property(propertyAccessor, "Value");
            Expression<Func<TEntity, object?>> valuePropertyAccessor = Expression.Lambda<Func<TEntity, object?>>(
                valueProperty,
                parameters);

            return _nullableValueUnwrapper.Unwrap(valuePropertyAccessor);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
