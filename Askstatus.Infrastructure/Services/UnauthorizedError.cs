using System.Net;

namespace Askstatus.Infrastructure.Services;
public class UnauthorizedError : BaseIdentityError
{
    public UnauthorizedError(string message)
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
