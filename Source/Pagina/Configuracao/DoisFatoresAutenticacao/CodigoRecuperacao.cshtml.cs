using System;
using System.Linq;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoVirtualEstudantilWeb.Pagina.Configuracao.DoisFatoresAutenticacao
{
    public class CodigoRecuperacaoModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private ApplicationDbContext DbContext { get; }
        public string[] RecoveryCodes { get; set; }

        public CodigoRecuperacaoModel(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            UserManager = userManager;
            DbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");

            if (!user.TwoFactorEnabled)
            {
                //throw new ApplicationException($"Cannot show recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");

                TempData["StatusMessage"] = $"Não é possível mostrar os códigos de recuperação para o usuário com o ID '{user.UserName}', pois eles não têm o autenticação de dois fatores ativado.";
                return RedirectToPage("./Configuracao");
            }

            //var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            //RecoveryCodes = recoveryCodes.ToArray();

            var userToken = DbContext.UserTokens.FirstOrDefault(token => token.UserId == UserManager.GetUserId(User) && token.Name == "RecoveryCodes" && token.LoginProvider == "[AspNetUserStore]");

            RecoveryCodes = userToken?.Value.Split(";");

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostGenerateNewAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            if (!user.TwoFactorEnabled)
            {
                TempData["StatusMessage"] = $"Não é possível mostrar códigos de recuperação para o usuário com ID '{user.UserName}' como eles não têm autenticação dois fatores ativado.";
                return RedirectToPage("./Config");
            }

            await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData["StatusMessage"] = "Você gerou novos códigos de recuperação para autenticação de dois fatores.";

            return RedirectToPage();
        }
    }
}