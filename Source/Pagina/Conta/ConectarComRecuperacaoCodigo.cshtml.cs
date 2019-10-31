using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Miscelanea;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class ConectarComRecuperacaoCodigoModel : PageModel
    {
        private SignInManager<ApplicationUser> SignInManager { get; }
        private ILogger<ConectarComRecuperacaoCodigoModel> Logger { get; }
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; private set; }

        public ConectarComRecuperacaoCodigoModel(SignInManager<ApplicationUser> signInManager, ILogger<ConectarComRecuperacaoCodigoModel> logger)
        {
            SignInManager = signInManager;
            Logger = logger;
        }

        public class InputModel
        {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Código de recuperação")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("./Conectar");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException("Não foi possível carregar o usuário de autenticação de dois fatores.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                Logger.LogInformation("Usuário com o ID '{UserId}' foi conectado com um código de recuperação.", user.Id);
                return LocalRedirect(Url.PegarUrlLocal(returnUrl));
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning("Usuário com o ID '{user.Id}' teve a conta bloqueada.", user.Id);
                return RedirectToPage("./Bloqueado");
            }

            Logger.LogWarning("Código de recuperação inserido está inválido para o usuário com o ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Código de recuperação inválido digitado.");
            return Page();
        }
    }
}
