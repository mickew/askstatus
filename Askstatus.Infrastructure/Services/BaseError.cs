using System.Net;
using FluentResults;

namespace Askstatus.Infrastructure.Services;
public class BaseError : Error
{
    public BaseError(string message, HttpStatusCode httpStatusCode)
        : base(message)
    {
        Metadata.Add("HttpStatusCode", httpStatusCode);
    }
}
