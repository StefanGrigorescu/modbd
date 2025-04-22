using System.Linq.Expressions;

namespace MODBD_Api.Common.Metadata;

public interface IUnwrapper
{
    Expression<Func<TEntity, object?>> Unwrap<TEntity>(Expression<Func<TEntity, object?>> propertyAccessor);
    Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters);

    public static Expression<Func<TEntity, object?>> UnwrapValueTypeOrReferenceType<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        return propertyAccessor.Type.IsValueType ?
            UnwrapValueType<TEntity>(propertyAccessor, parameters) :
            Expression.Lambda<Func<TEntity, object?>>(propertyAccessor, parameters);
    }

    public static Expression<Func<TEntity, object?>> UnwrapValueType<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        UnaryExpression convertToObject = Expression.Convert(
            propertyAccessor,
            typeof(object));

        return Expression.Lambda<Func<TEntity, object?>>(
            convertToObject,
            parameters);
    }

    public static Expression<Func<TEntity, object?>> UnwrapValueType<TEntity>(UnaryExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters)
    {
        UnaryExpression convertToObject = Expression.Convert(
            propertyAccessor,
            typeof(object));

        return Expression.Lambda<Func<TEntity, object?>>(
            convertToObject,
            parameters);
    }
}
