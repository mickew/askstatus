using FluentValidation;

namespace Askstatus.Common.System;

public sealed class SystemSendMailRequest
{
    public string EmailTo { get; set; }
    public string FirstName { get; set; }
    public string Header { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public SystemSendMailRequest()
    {
        EmailTo = string.Empty;
        FirstName = string.Empty;
        Header = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
    }

    public SystemSendMailRequest(string emailTo, string firstName, string header, string subject, string body)
    {
        EmailTo = emailTo;
        FirstName = firstName;
        Header = header;
        Subject = subject;
        Body = body;
    }
}

public class SystemSendMailRequestFluentValidator : AbstractValidator<SystemSendMailRequest>
{
    public SystemSendMailRequestFluentValidator()
    {
        RuleFor(x => x.EmailTo)
            .NotEmpty().WithMessage("Email To is required.")
            .EmailAddress().WithMessage("Email To must be a valid email address.");
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.");
        RuleFor(x => x.Header)
            .NotEmpty().WithMessage("Header is required.");
        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.");
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.");
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SystemSendMailRequest>.CreateWithOptions((SystemSendMailRequest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
