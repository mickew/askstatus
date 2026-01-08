using Askstatus.Application.Interfaces;
using Askstatus.Application.Users;
using Askstatus.Common.Models;
using Askstatus.Domain;
using Askstatus.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Options;

namespace Askstatus.Infrastructure.Events;
internal sealed class UserChangedEventHandler : INotificationHandler<UserChangedIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly IOptionsSnapshot<MailSettings> _options;

    public UserChangedEventHandler(IEmailService emailService, IOptionsSnapshot<MailSettings> options)
    {
        _emailService = emailService;
        _options = options;
    }
    public async Task Handle(UserChangedIntegrationEvent notification, CancellationToken cancellationToken)
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

    private async Task SendRegistrationConfirmationEmail(UserChangedIntegrationEvent notification)
    {
        var from = "info@askstatus.com";
        from = _options.Value.Account ?? from;

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

    private async Task SendResetPasswordEmail(UserChangedIntegrationEvent notification)
    {
        var from = "info@askstatus.com";
        from = _options.Value.Account ?? from;

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
