using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApiController : ControllerBase
{

    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count != 0)
        {
            return Problem(errors.First());
        }
        return Problem();
    }

    protected IActionResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description,
            extensions: error.Metadata is not null ? new Dictionary<string, object?> { { "errorDetails", error.Metadata } } : null
        );
    }

}