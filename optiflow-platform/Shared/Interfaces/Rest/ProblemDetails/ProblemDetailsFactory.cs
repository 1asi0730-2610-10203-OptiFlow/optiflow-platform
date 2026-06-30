using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace optiflow_platform.Shared.Interfaces.Rest.ProblemDetails;

/// <summary>
/// Factory for creating RFC 7807 ProblemDetails action results.
/// </summary>
public class ProblemDetailsFactory
{
    /// <summary>
    /// Creates a ProblemDetails ObjectResult with the given status, error, detail, and title.
    /// </summary>
    public ObjectResult CreateProblemDetails(
        ControllerBase controller,
        int statusCode,
        Enum? error,
        string detail,
        string title)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status  = statusCode,
            Title   = title,
            Detail  = detail,
            Type    = $"https://httpstatuses.com/{statusCode}"
        };

        if (error is not null)
            problemDetails.Extensions["errorCode"] = error.ToString();

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
