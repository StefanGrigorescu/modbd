using System.Runtime.CompilerServices;
using MODBD_Api.Common.Abstractions.DomainExceptions;

namespace MODBD_Api.Common.Text;

/// <summary>
/// Exception for leading or trailing white spaces.
/// </summary>
public sealed class LeadingOrTrailingWhiteSpacesException : ValueObjectException
{
    public static void ThrowIfHasLeadingOrTrailingWhiteSpaces(
        string text,
        [CallerArgumentExpression(nameof(text))] string textParamName = "'text'"
    ) {
        if(text.Trim() != text)
        {
            throw new LeadingOrTrailingWhiteSpacesException(textParamName);
        }
    }

    private LeadingOrTrailingWhiteSpacesException(string textParamName) :
        base($"$'{textParamName}' cannot start or end with -, _, or .")
    { }
}
