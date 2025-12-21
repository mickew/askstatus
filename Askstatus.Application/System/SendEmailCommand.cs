using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Domain;
using Askstatus.Domain.Constants;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Askstatus.Application.System;
public sealed record SendEmailCommand(string To, string Header, string FirstName, string Subject, string Body) : IRequest<Result>;

public sealed class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmailCommandHandler> _logger;
    private readonly IOptionsSnapshot<MailSettings> _options;

    public SendEmailCommandHandler(IEmailService emailService, ILogger<SendEmailCommandHandler> logger, IOptionsSnapshot<MailSettings> options)
    {
        _emailService = emailService;
        _logger = logger;
        _options = options;
    }
    public async Task<Result> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var body = MailMessageBody.SendMailBody(request.FirstName, request.Header, request.Body);
        var email = new MailMessage(_options.Value.Account!, request.To, "nousername", request.FirstName, request.Subject, body);
        var result = await _emailService.SendEmailAsync(email);
        return result == true ? Result.Ok() : Result.Fail(new ServerError("Failed to send email"));
    }
}
