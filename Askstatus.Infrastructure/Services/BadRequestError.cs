using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Askstatus.Infrastructure.Services;
public class BadRequestError : BaseIdentityError
{
    public BadRequestError(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestError(string message, IEnumerable<IdentityError> errors)
        : base(message, HttpStatusCode.BadRequest, errors)
    {
    }
}
