using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BancoVirtualEstudantilWeb.Data;

namespace BancoVirtualEstudantilWeb.Controllers
{
    [Route("[controller]/[action]")]
    public class ContaController : Controller
    {
        private SignInManager<ApplicationUser> SignInManager { get; }
        private ILogger Logger { get; }

        public ContaController(SignInManager<ApplicationUser> signInManager, ILogger<ContaController> logger)
        {
            SignInManager = signInManager;
            Logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            Logger.LogInformation("Usuário desconectado.");
            return RedirectToPage("/Index");
        }
    }
}
