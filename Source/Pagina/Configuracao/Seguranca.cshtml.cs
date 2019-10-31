using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BancoVirtualEstudantilWeb.Pagina.Configuracao
{
    public class SegurancaModel : PageModel
    {
        private ILogger<IndexModel> Logger { get; }
        private UserManager<ApplicationUser> UserManager { get; }
        private SignInManager<ApplicationUser> SignInManager { get; }
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationScheme> OtherLogins { get; set; }
        public bool ShowRemoveButton { get; set; }
        public bool Is2FaEnabled { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public bool HasAuthenticator { get; set; }

        public SegurancaModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<IndexModel> logger) 
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{ UserManager.GetUserId(User)}'.");

            CurrentLogins = await UserManager.GetLoginsAsync(user);
            OtherLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()) .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider)) .ToList();

            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;

            Is2FaEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
            RecoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(user);
            HasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(user) != null;

            return Page();
        }

        public IActionResult OnPost()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");
            }

            var result = await UserManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Ocorreu um erro inesperado ao remover o logon externo do usuário com o ID '{user.Id}'.");
            }

            await SignInManager.SignInAsync(user, false);

            TempData["StatusMessage"] = $"A conectividade externa para {loginProvider} foi removido.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./Seguranca", "LinkLoginCallback");
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserManager.GetUserId(User));

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            var info = await SignInManager.GetExternalLoginInfoAsync(await UserManager.GetUserIdAsync(user));
            if (info == null)
            {
                throw new ApplicationException($"Ocorreu um erro inesperado ao carregar informações de login externas para o usuário com ID '{user.Id}'.");
            }

            var result = await UserManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Ocorreu um erro inesperado ao adicionar logon externo para usuário com ID '{ user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            
            TempData["StatusMessage"] = $"A conectividade externa para {info.ProviderDisplayName} foi adicionada.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisable2FaAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            var disable2FaResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FaResult.Succeeded)
            {
                throw new ApplicationException($"Ocorreu um erro inesperado ao desativar a autenticação de dois fatores para o usuário com ID '{UserManager.GetUserId(User)}'.");
            }

            Logger.LogInformation("O usuário com o ID '{UserId}' desativou autenticação de dois fatores.", UserManager.GetUserId(User));

            return RedirectToPage();
        }
    }
}