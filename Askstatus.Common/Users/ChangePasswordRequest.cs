namespace Askstatus.Common.Users;
public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
