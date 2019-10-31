using Microsoft.AspNetCore.Mvc;

namespace BancoVirtualEstudantilWeb.Controllers
{
    public class ConfiguracaoController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToPage("/Configuracao/Perfil");
        }

        public IActionResult TwoFactorAuth()
        {
            return RedirectToPage("/Configuracao/DoisFatoresAutenticacao/Configuracao");
        }
    }
}