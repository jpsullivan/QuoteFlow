﻿using System.Net.Mail;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IMessageService
    {
        void SendNewAccountEmail(MailAddress toAddress, string confirmationUrl);
        void SendEmailChangeConfirmationNotice(MailAddress newEmailAddress, string confirmationUrl);
        void SendPasswordResetInstructions(User user, string resetPasswordUrl, bool forgotPassword);
        void SendEmailChangeNoticeToPreviousEmailAddress(User user, string oldEmailAddress);
        void SendCredentialRemovedNotice(User user, Credential removed);
        void SendCredentialAddedNotice(User user, Credential added);
    }
}