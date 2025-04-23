namespace MODBD_Common.NullableTypes;

public static class NullableTypesOrderBy
{
    /// <summary>
    /// If <paramref name="value"/> is null, it returns 1, otherwise 0. <br></br>
    /// Since OrderBy is ascending by default, 1 will be placed last.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int NullValuesLast<T>(T? value)
        where T : struct
        => value.HasValue ? 0 : 1;

    /// <summary>
    /// If <paramref name="value"/> is null, it returns 1, otherwise 0. <br></br>
    /// Since OrderBy is ascending by default, 1 will be placed last.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int NullValuesLast<T>(T? value)
        where T : class
    => value is null ? 1 : 0;

    /// <summary>
    /// If <paramref name="value"/> is null, it returns 0, otherwise 1. <br></br>
    /// Since OrderBy is ascending by default, 0 will be placed first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int NullValuesFirst<T>(T? value)
        where T : struct
        => value.HasValue ? 1 : 0;

    /// <summary>
    /// If <paramref name="value"/> is null, it returns 0, otherwise 1. <br></br>
    /// Since OrderBy is ascending by default, 0 will be placed first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int NullValuesFirst<T>(T? value)
        where T : class
        => value is null ? 0 : 1;
}
