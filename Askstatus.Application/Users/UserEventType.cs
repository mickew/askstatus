using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Askstatus.Application.Users;
public enum UserEventType
{
    None = 0,
    UserCreated = 1,
    UserUpdated = 2,
    UserDeleted = 3,
    UserPasswordReset = 4,
    UserPasswordChanged = 5,
    UserAccessControlUpdated = 6,
    RoleCreated = 7,
    RoleUpdated = 8,
    RoleDeleted = 9,
    UserEmailConfirm = 10,
    UserForgotPassword = 11,
    UserPasswordUserReset = 12,
}
