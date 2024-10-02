using System.Net;
using System.Text;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;

public class AskstatusAspNetCoreResultEndpointProfile : IAspNetCoreResultEndpointProfile
{
    private readonly HttpStatusCode _statusCode;

    public AskstatusAspNetCoreResultEndpointProfile(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
    }

    public ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        ResultBase result = context.Result;
        var detail = result.Errors.Aggregate(new StringBuilder(), (sb, a) => sb.AppendLine(String.Join(",", a.Message)), sb => sb.ToString()).TrimEnd('\r', '\n');
        var problem = new ProblemDetails
        {
            Title = _statusCode.ToString(),
            Detail = detail,
            Status = (int)_statusCode,
            Type = $"https://httpstatuses.com/{(int)_statusCode}"
        };
        switch (_statusCode)
        {
            case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(problem);
            case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult(problem);
            case HttpStatusCode.NotFound:
                return new NotFoundObjectResult(problem);
            default:
                return new BadRequestObjectResult(problem);
        }
    }

    public ActionResult TransformOkNoValueResultToActionResult(OkResultToActionResultTransformationContext<Result> context)
    {
        return new OkResult();
    }

    public ActionResult TransformOkValueResultToActionResult<T>(OkResultToActionResultTransformationContext<Result<T>> context)
    {
        return new OkObjectResult(context.Result.ValueOrDefault);
    }
}
