using System.Net;
using Askstatus.Application.Interfaces;

namespace Askstatus.Infrastructure.Services;
internal class ApplicationHostAddressService : IApplicationHostAddressService
{
    private string? _ipAddress;

    public string IpAddress
    {
        get
        {
            if (string.IsNullOrEmpty(_ipAddress))
            {
                _ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)!
                    .ToString();
            }
            return _ipAddress;
        }
    }
}
