namespace BudgetFlow.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string body,
            byte[]? attachment = null,
            string? attachmentName = null
        );
    }
}