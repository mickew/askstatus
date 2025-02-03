namespace Askstatus.Common.Models;
public sealed record MailMessage(string From, string To, string UserName, string FirstName, string Subject, MailBody Body);

public sealed record MailBody(string Text, string Html);
