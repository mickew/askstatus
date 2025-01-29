namespace Askstatus.Common.Users;
public sealed record ResetPasswordRequest(string UserId, string Token, string NewPassword);
