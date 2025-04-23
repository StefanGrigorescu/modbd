using System.Linq.Expressions;

namespace MODBD_Common.Metadata;

/// <summary>
/// This is a default unwrapper. It does not need to inherit from <see cref="Unwrapper"/>. <br></br>
/// It will not be instantiated outside its class definition, but only be used as the end in an unwrappers chain. 
/// </summary>
public sealed class DefaultUnwrapper : IUnwrapper
{
    public Expression<Func<TEntity, object?>> Unwrap<TEntity>(Expression<Func<TEntity, object?>> propertyAccessor)
        => propertyAccessor;

    /// <summary>
    ///  This is the default property accessor, the unit property accessor. It does not change the <paramref name="propertyAccessor"/>, but returns it as it is instead. 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="propertyAccessor"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Expression<Func<TEntity, object?>> Unwrap<TEntity>(MemberExpression propertyAccessor, IReadOnlyCollection<ParameterExpression> parameters) =>
        IUnwrapper.UnwrapValueTypeOrReferenceType<TEntity>(
            propertyAccessor,
            parameters);

    public static readonly DefaultUnwrapper Instance = new();

    private DefaultUnwrapper() { }
}
