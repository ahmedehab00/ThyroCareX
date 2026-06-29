using System.Threading.Tasks;

namespace ThyroCareX.Service.Abstarct
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
