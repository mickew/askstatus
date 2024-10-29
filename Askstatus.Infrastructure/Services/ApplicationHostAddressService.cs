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
                    .Where(x => x.ToString() != "127.0.1.1")
                    .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)!
                    .ToString();
            }
            return _ipAddress;
        }
    }
}
