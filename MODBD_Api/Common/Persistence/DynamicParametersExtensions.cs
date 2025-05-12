using Dapper;
using MODBD_Common.Abstractions;
using System.Data;

namespace MODBD_Api.Common.Persistence;

public static class DynamicParametersExtensions
{
    public static DynamicParameters WithParameter(
        this DynamicParameters parameters,
        string name, 
        object? value = null,
        DbType? dbType = null,
        ParameterDirection? direction = null
    ) {
        parameters.Add(name, value, dbType, direction);
        return parameters;
    }

    public static DynamicParameters WithParameter<TValue>(
        this DynamicParameters parameters,
        string name, 
        ValueObject<TValue> value,
        DbType? dbType = null,
        ParameterDirection? direction = null
    )
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        parameters.Add(name, value.Value, dbType, direction);
        return parameters;
    }
}
