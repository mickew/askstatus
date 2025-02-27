using System.Net;

namespace Askstatus.Application.Errors;

public class ServerError : BaseError
{
    public ServerError(string message) : base(message, HttpStatusCode.InternalServerError)
    {
    }
}
