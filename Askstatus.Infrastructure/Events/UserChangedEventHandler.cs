using Askstatus.Application.Interfaces;
using Askstatus.Application.Users;
using Askstatus.Common.Models;
using Askstatus.Domain;
using Askstatus.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Options;

namespace Askstatus.Infrastructure.Events;
internal sealed class UserChangedEventHandler : INotificationHandler<UserChangedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IOptions<AskstatusApiSettings> _apiOptions;

    public UserChangedEventHandler(IEmailService emailService, IOptions<AskstatusApiSettings> apiOptions)
    {
        _emailService = emailService;
        _apiOptions = apiOptions;
    }
    public async Task Handle(UserChangedEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.EventType)
        {
            case UserEventType.UserCreated:
                await SendRegistrationConfirmationEmail(notification);
                break;
            case UserEventType.UserForgotPassword:
                await SendResetPasswordEmail(notification);
                break;
            // Add other cases here if needed
            default:
                break;
        }
    }

    private async Task SendRegistrationConfirmationEmail(UserChangedEvent notification)
    {
        var uri = new Uri(_apiOptions.Value.FrontendUrl!);
        var from = $"info@{uri.Host}";

        var mailMessage = new MailMessage
        (
            from,
            notification.User.Email,
            notification.User.UserName,
            notification.User.FirstName,
            $"Welcome to AskStatus {notification.User.FirstName}",
            MailMessageBody.RegistrationConfirmationMailBody(notification.User.UserName, notification.User.Link!, notification.User.FirstName)
        );

        await _emailService.SendEmailAsync(mailMessage);
    }

    private async Task SendResetPasswordEmail(UserChangedEvent notification)
    {
        var uri = new Uri(_apiOptions.Value.FrontendUrl!);
        var from = $"info@{uri.Host}";

        var mailMessage = new MailMessage
        (
            from,
            notification.User.Email,
            notification.User.UserName,
            notification.User.FirstName,
            $"Askstatus reset password request for {notification.User.UserName}",
            MailMessageBody.ResetPasswordMailBody(notification.User.Link!, notification.User.FirstName)
        );

        await _emailService.SendEmailAsync(mailMessage);
    }
}
