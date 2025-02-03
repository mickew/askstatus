using System.Reflection;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace Askstatus.Infrastructure.Mail;
public sealed class MailKitSmtpClient : IAskStatusSmtpClient
{
    private readonly ILogger<MailKitSmtpClient> _logger;
    private readonly IOptions<MailSettings> _options;

    public MailKitSmtpClient(ILogger<MailKitSmtpClient> logger, IOptions<MailSettings> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<bool> SendEmailAsync(MailMessage mailMessage)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(mailMessage.From));
        message.To.Add(MailboxAddress.Parse(mailMessage.To));
        message.Subject = mailMessage.Subject;

        var builder = new BodyBuilder();
        builder.TextBody = mailMessage.Body.Text;

        MimeEntity image;
        using (var stream = FindResource(names => names?.FirstOrDefault(rn => rn.Contains("logo-32x32.png"))))
        {
            image = builder.LinkedResources.Add("logo-32x32.png", stream);
            image.ContentId = MimeUtils.GenerateMessageId();
        }

        builder.HtmlBody = mailMessage.Body.Html.Replace("{imageCid}", image.ContentId);
        message.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                SaslMechanismOAuth2 oauth2 = null!;
                if (_options.Value.Host!.EndsWith(".gmail.com"))
                {
                    oauth2 = await GmailOauth2Async();
                }
                await client.ConnectAsync(_options.Value.Host, _options.Value.Port, _options.Value.EnableSsl);
                if (_options.Value.Host!.EndsWith(".gmail.com"))
                {
                    await client.AuthenticateAsync(oauth2);
                }
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError("Error trying to connect: {0}", ex.Message);
                _logger.LogError("\tStatusCode: {0}", ex.StatusCode);
                return false;
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError("Protocol error while trying to connect: {0}", ex.Message);
                return false;
            }

            // Note: Not all SMTP servers support authentication, but GMail does.
            //if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            //{
            //    try
            //    {
            //        client.Authenticate("username", "password");
            //    }
            //    catch (AuthenticationException)
            //    {
            //        _logger.LogError("Invalid user name or password.");
            //        return false;
            //    }
            //    catch (SmtpCommandException ex)
            //    {
            //        _logger.LogError("Error trying to authenticate: {Message}", ex.Message);
            //        _logger.LogError("\tStatusCode: {StatusCode}", ex.StatusCode);
            //        return false;
            //    }
            //    catch (SmtpProtocolException ex)
            //    {
            //        _logger.LogError("Protocol error while trying to authenticate: {Message}", ex.Message);
            //        return false;
            //    }
            //}

            try
            {
                var result = await client.SendAsync(message);
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError("Error sending message: {Message}", ex.Message);
                _logger.LogError("\tStatusCode: {StatusCode}", ex.StatusCode);

                switch (ex.ErrorCode)
                {
                    case SmtpErrorCode.RecipientNotAccepted:
                        _logger.LogError("\tRecipient not accepted: {Mailbox}", ex.Mailbox);
                        break;
                    case SmtpErrorCode.SenderNotAccepted:
                        _logger.LogError("\tSender not accepted: {Mailbox}", ex.Mailbox);
                        break;
                    case SmtpErrorCode.MessageNotAccepted:
                        _logger.LogError("\tMessage not accepted.");
                        break;
                }
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError("Protocol error while sending message: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Protocol error while sending message: {Message}", ex.Message);
            }

            client.Disconnect(true);
        }
        return true;
    }

    private static Stream? FindResource(Func<string[]?, string?> finder)
    {
        foreach (var assembly in AllAssembliesOfCurrentAppDomain)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = finder(resourceNames);

            if (resourceName is not null)
            {
                Console.WriteLine($"Resource {resourceName} found in {assembly.FullName}");
                return assembly.GetManifestResourceStream(resourceName);
            }
        }

        return null;
    }

    private static Assembly[] AllAssembliesOfCurrentAppDomain
        => AppDomain.CurrentDomain.GetAssemblies();

    private async Task<SaslMechanismOAuth2> GmailOauth2Async()
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = _options.Value.ClientId,
            ClientSecret = _options.Value.ClientSecret
        };
        var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            // Cache tokens in ~/.local/share/google-filedatastore/CredentialCacheFolder on Linux/Mac
            DataStore = new FileDataStore(_options.Value.CredentialCacheFolder, true),
            Scopes = new[] { "https://mail.google.com/" },
            ClientSecrets = clientSecrets
        });
        var codeReceiver = new LocalServerCodeReceiver();
        var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);
        var credential = await authCode.AuthorizeAsync(_options.Value.Account, CancellationToken.None);

        // Refresh the token if it has expired
        if (credential.Token.IsStale)
        {
            await credential.RefreshTokenAsync(CancellationToken.None);
        }

        SaslMechanismOAuth2 oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);
        return oauth2;
    }
}
