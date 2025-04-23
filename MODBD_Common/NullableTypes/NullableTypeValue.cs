using System.Runtime.CompilerServices;
using MODBD_Common.Abstractions.Responses;

namespace MODBD_Common.NullableTypes;

public static class NullableTypeValue
{
    public static AppResponse<object> UnwrapValueIfIsOfNullableType(
        object instance,
        [CallerArgumentExpression(nameof(instance))] string instanceParamName = "'instance'")
    {
        if (instance is not ValueType)
        {
            return AppResponse<object>.Failed($"Parameter {instanceParamName} type is not a value type!");
        }

        Type type = instance.GetType();
        if (!type.IsGenericType ||
            type.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
            return AppResponse<object>.Failed($"Parameter {instanceParamName} type is not a generic Nullable<T> type!");
        }

        bool hasValue = type
            .GetProperty("HasValue")?
            .GetValue(instance) 
            as bool? ??
            false;
        if (!hasValue)
        {
            return AppResponse<object>.Failed($"Parameter {instanceParamName} does not have any value (i.e. is null)!");
        }

        // Access value via reflection
        object underlyingValue = type
            .GetProperty("Value")?
            .GetValue(instance)!;

        return AppResponse<object>.Succeeded(underlyingValue);
    }
}
