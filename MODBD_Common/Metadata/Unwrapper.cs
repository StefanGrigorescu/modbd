using System.Linq.Expressions;

namespace MODBD_Common.Metadata;

/// <summary>
/// Base class for unwrappers.
/// </summary>
/// <param name="nextUnwrapper"></param>
public abstract class Unwrapper(IUnwrapper nextUnwrapper) : IUnwrapper
{
    protected readonly IUnwrapper _nextUnwrapper = nextUnwrapper;

    public Expression<Func<TEntity, object?>> Unwrap<TEntity>(Expression<Func<TEntity, object?>> propertyAccessor)
    {
        return propertyAccessor.Body is MemberExpression memberExpression ?
            Unwrap<TEntity>(memberExpression, propertyAccessor.Parameters) :
            _nextUnwrapper.Unwrap(propertyAccessor);
    }

    public abstract Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters);
}
