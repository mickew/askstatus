namespace Askstatus.Common.Users;
public sealed record ConfirmEmailRequest(string UserId, string Token);
