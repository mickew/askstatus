using System.Net;

namespace Askstatus.Application.Errors;
public sealed class BadRequestError : BaseError
{
    public BadRequestError(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}
