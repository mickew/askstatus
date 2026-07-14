using Askstatus.Common.Users;

namespace Askstatus.Application.Users;

public sealed class UserVMWithLink : UserVM
{
    public UserVMWithLink() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false) { }
    public UserVMWithLink(string id, string userName, string email, string firstName, string lastName, string link, bool isLockedOut)
      : base(id, userName, email, firstName, lastName, isLockedOut)
    {
        Link = link;
    }

    public string? Link { get; set; }

}
