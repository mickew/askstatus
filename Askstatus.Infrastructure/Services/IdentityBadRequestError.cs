using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Askstatus.Infrastructure.Services;
public class IdentityBadRequestError : BaseIdentityError
{
    public IdentityBadRequestError(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }

    public IdentityBadRequestError(string message, IEnumerable<IdentityError> errors)
        : base(message, HttpStatusCode.BadRequest, errors)
    {
    }
}
