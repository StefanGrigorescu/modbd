using System.Text;

namespace MODBD_Common.Text;

public static class StringFormatters
{
    public static string FormatFromPascalToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        if (input.Length == 1)
        {
            return input.ToLowerInvariant();
        }

        StringBuilder sb = new(input.Length);
        sb.Append(char.ToLowerInvariant(input[0]));
        sb.Append(input.AsSpan(1));

        return sb.ToString();
    }
}
