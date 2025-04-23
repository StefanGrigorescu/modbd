using System.Linq.Expressions;
using MODBD_Common.Abstractions.DiscriminatedUnions;

namespace MODBD_Common.Metadata;

public sealed class EnumUnwrapper(IUnwrapper nextUnwrapper) : Unwrapper(nextUnwrapper)
{
    public override Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        if (propertyAccessor.Type.IsEnum)
        {
            // Explicitly cast enum to int
            UnaryExpression convertToInt = Expression.Convert(propertyAccessor, typeof(int));

            return IUnwrapper.UnwrapValueType<TEntity>(convertToInt, parameters);
        }

        // Check if the type inherits from Enumeration<TEnum>
        if (propertyAccessor.Type.IsAssignableTo(typeof(Enumeration<>)))
        {
            // Explicitly cast enum to int
            MemberExpression convertToInt = Expression.Property(propertyAccessor, "Id");

            return IUnwrapper.UnwrapValueType<TEntity>(convertToInt, parameters);
        }

        return _nextUnwrapper.Unwrap<TEntity>(propertyAccessor, parameters);
    }
}
