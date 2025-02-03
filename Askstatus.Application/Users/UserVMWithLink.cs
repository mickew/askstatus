using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Askstatus.Common.Users;

namespace Askstatus.Application.Users;
public sealed class UserVMWithLink : UserVM
{
    public UserVMWithLink() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }
    public UserVMWithLink(string id, string userName, string email, string firstName, string lastName, string link)
      :  base(id, userName, email, firstName, lastName)
    {
        Link = link;
    }

    public string? Link { get; set; }

}
