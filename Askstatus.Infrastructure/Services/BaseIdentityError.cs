using System.Net;
using Askstatus.Application.Errors;
using Microsoft.AspNetCore.Identity;

namespace Askstatus.Infrastructure.Services;
public class BaseIdentityError : BaseError
{
    public BaseIdentityError(string message, HttpStatusCode httpStatusCode)
        : base(message, httpStatusCode)
    {
    }

    public BaseIdentityError(string message, HttpStatusCode httpStatusCode, IEnumerable<IdentityError> errors)
        : base(message, httpStatusCode)
    {
        foreach (var error in errors)
        {
            Metadata.Add(error.Description, error.Code);
        }
    }
}
