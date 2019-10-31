using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Miscelanea;
using BancoVirtualEstudantilWeb.Services.Mail;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class SenhaEsquecidaModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private IMailManager EmailSender { get; }
        [BindProperty]
        public InputModel Input { get; set; }

        public SenhaEsquecidaModel(UserManager<ApplicationUser> userManager, IMailManager emailSender)
        {
            UserManager = userManager;
            EmailSender = emailSender;
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToPage("./ConfirmacaoSenhaEsquecida");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.PegarUrlRedefinicaoSenha(user.Id, code, Request.Scheme);
                await EmailSender.EnviarRedifinicaoSenhaAsync(Input.Email, callbackUrl);
                return RedirectToPage("./ConfirmacaoSenhaEsquecida");
            }

            return Page();
        }
    }
}
