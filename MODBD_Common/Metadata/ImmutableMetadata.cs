using System.Diagnostics;
using System.Reflection;
using MODBD_Common.Abstractions.Responses;
using MODBD_Common.Collections;

namespace MODBD_Common.Metadata;

public static class MaybeImmutableType
{
    public static bool IsMutableType(Type type) =>
        type.IsPrimitive ||
        IsACollectionOfMutableTypes(type) ||
        HasAnyNonInitOnlySetter(type) ||
        HasAnyMutableTypeProperty(type) ||
        HasAnyMutableCollectionProperty(type) ||
        HasAnyCollectionOfMutableTypeProperty(type);


    public static bool IsACollectionOfMutableTypes(Type type) =>
        type.IsACollectionType() &&
        type
            .GetGenericArguments()
            .Any(IsMutableType);


    public static bool HasAnyNonInitOnlySetter(Type type) =>
    type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select((PropertyInfo property) => property.IsInitOnly())
        .Where(response => response.IsSuccess)
        .Select((AppResponse<bool> response) => {
            return response.TryGetData(out bool isInitOnly) ?
                isInitOnly :
                throw new UnreachableException("Response was expected to be successful.");
        }).Any((bool isInitOnly) => isInitOnly is false);

    public static AppResponse<bool> IsInitOnly(this PropertyInfo property)
    {
        MethodInfo? setMethod = property.SetMethod;
        if (setMethod is null)
        {
            return AppResponse<bool>.Failed("Property has no setter");
        }

        Type isExternalInitType = typeof(System.Runtime.CompilerServices.IsExternalInit);
        return AppResponse<bool>.Succeeded(
            setMethod
                .ReturnParameter
                .GetRequiredCustomModifiers()
                .Contains(isExternalInitType)
        );
    }


    public static bool HasAnyMutableTypeProperty(Type type) => type
        .GetProperties(BindingFlags.Instance)
        .Any((PropertyInfo property) =>
            IsMutableType(property.PropertyType)
        );


    public static bool HasAnyMutableCollectionProperty(Type type) => type
        .GetProperties(BindingFlags.Instance)
        .Any((PropertyInfo property) => 
            property.PropertyType.IsAMutableCollectionType()
        );


    public static bool HasAnyCollectionOfMutableTypeProperty(Type type) => type
        .GetProperties(BindingFlags.Instance)
        .Any((PropertyInfo property) =>
            IsACollectionOfMutableTypes(property.PropertyType)
        );
}
