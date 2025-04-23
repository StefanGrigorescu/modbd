using Microsoft.AspNetCore.Mvc;
using MODBD_Common.Collections;

namespace MODBD_Api.Common.Contracts;

public sealed class ApiProblemDetails : ProblemDetails
{
    public required IReadOnlyList<string> ErrorMessages { get; init; }
    public required MetadataCollection ErrorMetadata { get; init; }
}
