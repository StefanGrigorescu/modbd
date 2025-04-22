namespace MODBD_Api.Common.Text;

public static class Csids
{
    public static readonly char Separator = ';';

    public static string Join<T>(IReadOnlyList<T> ids) =>
        string.Join(Separator, ids);

    public static string Join<T>(IEnumerable<T> ids) =>
        string.Join(Separator, ids);

    public static string[] Split(string csids) =>
        csids.Split(Separator);
}
