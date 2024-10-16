using System.Net;

namespace Askstatus.Infrastructure.Services;
public class NotFoundError : BaseIdentityError
{
    public NotFoundError(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}
