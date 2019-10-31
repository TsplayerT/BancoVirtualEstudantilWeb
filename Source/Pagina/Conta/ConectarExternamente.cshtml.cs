using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Miscelanea;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class ConectarExternamenteModel : PageModel
    {
        private SignInManager<ApplicationUser> SignInManager { get; }
        private UserManager<ApplicationUser> UserManager { get; }
        private ILogger<ConectarExternamenteModel> Logger { get; }
        [BindProperty]
        public InputModel Input { get; set; }
        public string LoginProvider { get; private set; }
        public string ReturnUrl { get; private set; }

        public ConectarExternamenteModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ConectarExternamenteModel> logger)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            Logger = logger;
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Conectar");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ConectarExternamente", "Callback", new { returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return RedirectToPage("./Conectar");
            }
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Conectar");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded)
            {
                Logger.LogInformation("{Name} efetuou login com o provedor {LoginProvider}.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(Url.PegarUrlLocal(returnUrl));
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Bloqueado");
            }

            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Erro ao carregar informações de login externas durante a confirmação.");
                }
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false);
                        Logger.LogInformation("O usuário criou uma conta usando o provedor {Name}.", info.LoginProvider);
                        return LocalRedirect(Url.PegarUrlLocal(returnUrl));
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
