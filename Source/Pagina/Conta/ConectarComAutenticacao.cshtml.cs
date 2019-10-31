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
    public class ConectarComAutenticacaoModel : PageModel
    {
        private SignInManager<ApplicationUser> SignInManager { get; }
        private ILogger<ConectarComAutenticacaoModel> Logger { get; }
        [BindProperty]
        public InputModel Input { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; private set; }

        public ConectarComAutenticacaoModel(SignInManager<ApplicationUser> signInManager, ILogger<ConectarComAutenticacaoModel> logger)
        {
            SignInManager = signInManager;
            Logger = logger;
        }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = " {0} deve ter pelo menos {2} e no m�ximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "C�digo do autenticador")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Lembre-se desta m�quina")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return RedirectToPage("./Conectar");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
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

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                Logger.LogInformation("Usu�rio com o ID '{UserId}' conectado com autentica��o de dois fatores", user.Id);
                return LocalRedirect(Url.PegarUrlLocal(returnUrl));
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning("Usu�rio com a conta ID '{UserId}' bloqueada.", user.Id);
                return RedirectToPage("./Bloqueado");
            }

            Logger.LogWarning("C�digo de autenticador inv�lido inserido para o usu�rio com o ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "C�digo do autenticador inv�lido.");
            return Page();
        }  
    }
}
