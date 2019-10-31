using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BancoVirtualEstudantilWeb.Data;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class ConfirmacaoEmailModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }

        public ConfirmacaoEmailModel(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"ão foi possível carregar o usuário com o ID '{userId}'.");
            }

            var result = await UserManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Erro ao confirmar o e-mail do usuário com ID '{userId}':");
            }

            return Page();
        }
    }
}
