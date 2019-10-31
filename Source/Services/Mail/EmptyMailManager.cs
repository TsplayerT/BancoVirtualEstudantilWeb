using System.Threading.Tasks;

namespace BancoVirtualEstudantilWeb.Services.Mail
{
    public class EmptyMailManager : IMailManager
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
