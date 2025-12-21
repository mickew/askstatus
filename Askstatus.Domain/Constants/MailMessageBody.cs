using System.Web;
using Askstatus.Common.Models;

namespace Askstatus.Domain.Constants;
public static class MailMessageBody
{
    private const string MailMessageBodyTxt = "Hello, this is a test email from Askstatus. If you received this email, it means that the email service is working correctly. Thank you for using Askstatus.";
    private const string MailMessageBodyHtml = "<html><body><p>Hello, this is a test email from Askstatus. If you received this email, it means that the email service is working correctly. Thank you for using Askstatus.</p></body></html>";

    private const string ResetPasswordTxt = @"
    Hi {name},

    You recently requested to reset your password for your Askstatus account. Click the link below to reset it:
    {resetLink}

    If you did not request a password reset, please ignore this email or reply to let us know. This password reset link is only valid for the next 24 hours.

    Thanks,
    The Askstatus Team";

    private const string ResetPasswordHtml = @"
    <html>
    <body>
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <div>
                <h2 style='color: #333;'><img src='cid:{imageCid}' alt='Askstatus Logo' style='max-width: 100%; height: auto;' /> Askstatus</h2>
            </div>
            <h3 style='color: #333;'>Reset Your Password</h3>
            <p>Hi {name},</p>
            <p>You recently requested to reset your password for your Askstatus account. Click the button below to reset it.</p>
            <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; margin: 20px 0; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>Reset your password</a>
            <p>If you did not request a password reset, please ignore this email or reply to let us know. This password reset link is only valid for the next 24 hours.</p>
            <p>Thanks,<br>The Askstatus Team</p>
        </div>
    </body>
    </html>";

    private const string RegistrationConfirmationTxt = @"
    Hi {name},

    Welcome to Askstatus! Your account with username {userName} has been successfully created. Click the link below to confirm your email address:
    {confirmationLink}

    If you did not create an account, please ignore this email or reply to let us know.

    Thanks,
    The Askstatus Team";

    private const string RegistrationConfirmationHtml = @"
    <html>
    <body>
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <div>
                <h2 style='color: #333;'><img src='cid:{imageCid}' alt='Askstatus Logo' style='max-width: 100%; height: auto;' /> Askstatus</h2>
            </div>
            <h3 style='color: #333;'>Welcome to Askstatus!</h3>
            <p>Hi {name},</p>
            <p>Welcome to Askstatus! Your account with username <b>{userName}</b> has been successfully created. Click the button below to confirm your email address.</p>
            <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; margin: 20px 0; font-size: 16px; color: #fff; background-color: #28a745; text-decoration: none; border-radius: 5px;'>Confirm your email</a>
            <p>If you did not create an account, please ignore this email or reply to let us know.</p>
            <p>Thanks,<br>The Askstatus Team</p>
        </div>
    </body>
    </html>";

    private const string SendMailTxt = @"
    Hi {name},
    
    {header}!
    {body}

    Thanks,
    The Askstatus Team";

    private const string SendMailHtml = @"
    <html>
    <body>
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <div>
                <h2 style='color: #333;'><img src='cid:{imageCid}' alt='Askstatus Logo' style='max-width: 100%; height: auto;' /> Askstatus</h2>
            </div>
            <h3 style='color: #333;'>{header}!</h3>
            <p>Hi {name},</p>
            <p>{body}</p>
            <p>Best regards,<br>The Askstatus Team</p>
        </div>
    </body>
    </html>";



    public static MailBody MailBodyTest() => new MailBody(MailMessageBodyTxt, MailMessageBodyHtml);

    public static MailBody ResetPasswordMailBody(string resetLink, string name)
    {
        string resetPasswordHtmlWithLink = ResetPasswordHtml.Replace("{resetLink}", resetLink).Replace("{name}", name);
        string resetPasswordTxtWithLink = ResetPasswordTxt.Replace("{resetLink}", resetLink).Replace("{name}", name);
        return new MailBody(resetPasswordTxtWithLink, resetPasswordHtmlWithLink);
    }

    public static MailBody RegistrationConfirmationMailBody(string userName, string confirmationLink, string name)
    {
        string registrationConfirmationHtmlWithLink = RegistrationConfirmationHtml.Replace("{userName}", userName).Replace("{confirmationLink}", confirmationLink).Replace("{name}", name);
        string registrationConfirmationTxtWithLink = RegistrationConfirmationTxt.Replace("{userName}", userName).Replace("{confirmationLink}", confirmationLink).Replace("{name}", name);
        return new MailBody(registrationConfirmationTxtWithLink, registrationConfirmationHtmlWithLink);
    }

    public static MailBody SendMailBody(string name, string header, string body)
    {
        string sendEmailHtml = SendMailHtml.Replace("{name}", name).Replace("{header}", header).Replace("{body}", HttpUtility.HtmlEncode(body).Replace("\n", "<br>"));
        string sendEmailTxt = SendMailTxt.Replace("{name}", name).Replace("{header}", header).Replace("{body}", body.Replace("\n", "\n    "));
        return new MailBody(sendEmailTxt, sendEmailHtml);
    }
}
