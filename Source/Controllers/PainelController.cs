using Microsoft.AspNetCore.Mvc;

namespace BancoVirtualEstudantilWeb.Controllers
{
    public class PainelController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Versao1");
        }

        public IActionResult Versao1()
        {
            return View();
        }

        public IActionResult Versao2()
        {
            return View();
        }

    }
}