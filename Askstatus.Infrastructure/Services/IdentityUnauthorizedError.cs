using System.Net;

namespace Askstatus.Infrastructure.Services;
public class IdentityUnauthorizedError : BaseIdentityError
{
    public IdentityUnauthorizedError(string message)
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
