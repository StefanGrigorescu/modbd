using System.Linq.Expressions;

namespace MODBD_Common.Metadata;

public static class ExpressionExtensions
{
    public static BinaryExpression IsPropertyNull(MemberExpression property)
    {
        ConstantExpression nullProperty = Expression.Constant(
                null,
                property.Type);

        return Expression.Equal(
            property,
            nullProperty);
    }
}
