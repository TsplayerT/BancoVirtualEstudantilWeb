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
            [Display(Name = "C�digo de recupera��o")]
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
                throw new ApplicationException("N�o foi poss�vel carregar o usu�rio de autentica��o de dois fatores.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                Logger.LogInformation("Usu�rio com o ID '{UserId}' foi conectado com um c�digo de recupera��o.", user.Id);
                return LocalRedirect(Url.PegarUrlLocal(returnUrl));
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning("Usu�rio com o ID '{user.Id}' teve a conta bloqueada.", user.Id);
                return RedirectToPage("./Bloqueado");
            }

            Logger.LogWarning("C�digo de recupera��o inserido est� inv�lido para o usu�rio com o ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "C�digo de recupera��o inv�lido digitado.");
            return Page();
        }
    }
}
