using System.Net;

namespace Askstatus.Infrastructure.Services;
public class IdentityNotFoundError : BaseIdentityError
{
    public IdentityNotFoundError(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}
