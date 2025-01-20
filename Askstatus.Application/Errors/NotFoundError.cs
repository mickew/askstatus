using System.Net;

namespace Askstatus.Application.Errors;
public sealed class NotFoundError : BaseError
{
    public NotFoundError(string message) : base(message, HttpStatusCode.NotFound)
    {
    }
}
