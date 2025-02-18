namespace Askstatus.Common.System;

public sealed record SystemInfoDto(
    SystemMailSettingsDto MailSettings,
    SystemApiSettingsDto ApiSettings);

public sealed record SystemMailSettingsDto(
    bool Enabled,
    string Host,
    int Port,
    string Account,
    string Password,
    string ClientId,
    string ClientSecret,
    bool EnableSsl,
    string CredentialCacheFolder);

public sealed record SystemApiSettingsDto(
    string BackendUrl,
    string FrontendUrl);
