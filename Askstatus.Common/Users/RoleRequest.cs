using Askstatus.Common.Authorization;

namespace Askstatus.Common.Users;
public sealed record RoleRequest(string Id, string Name, Permissions Permission);
