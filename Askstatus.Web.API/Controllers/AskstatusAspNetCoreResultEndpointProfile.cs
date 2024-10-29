using System.Net;
using System.Text;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;

public class AskstatusAspNetCoreResultEndpointProfile : IAspNetCoreResultEndpointProfile
{
    private const HttpStatusCode DefaultStatusCode = HttpStatusCode.BadRequest;

    public ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var satatusCode = DefaultStatusCode;
        ResultBase result = context.Result;
        var detail = result.Errors.Aggregate(new StringBuilder(), (sb, a) => sb.AppendLine(String.Join(",", a.Message)), sb => sb.ToString()).TrimEnd('\r', '\n');

        if (result.Errors.Count > 0)
        {
            if (result.Errors[0].Metadata.TryGetValue("HttpStatusCode", out var httpStatusCode))
            {
                satatusCode = (HttpStatusCode)httpStatusCode;
            }
        }

        var problem = new ProblemDetails
        {
            Title = satatusCode.ToString(),
            Detail = detail,
            Status = (int)satatusCode,
            Type = $"https://httpstatuses.com/{(int)satatusCode}"
        };
        switch (satatusCode)
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
