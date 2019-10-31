using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Services.Mail;

namespace BancoVirtualEstudantilWeb.Miscelanea
{
    public static class UtilitarioEmail
    {
        public static Task EnviarEmailConfirmacaoAsync(this IMailManager gerenciador, string email, string url)
        {
            return gerenciador.SendEmailAsync(email, "Confirmar email", $"Por favor confirme sua conta <a href='{HtmlEncoder.Default.Encode(url)}'>clicando aqui</a>.");
        }

        public static Task EnviarRedifinicaoSenhaAsync(this IMailManager gerenciador, string email, string url)
        {
            return gerenciador.SendEmailAsync(email, "Redefinir senha", $"Por favor redefina sua senha <a href='{HtmlEncoder.Default.Encode(url)}'>clicando aqui</a>.");
        }
    }
}
