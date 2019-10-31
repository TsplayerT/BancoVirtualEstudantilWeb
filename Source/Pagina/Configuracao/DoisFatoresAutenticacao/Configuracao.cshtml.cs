using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BancoVirtualEstudantilWeb.Pagina.Configuracao.DoisFatoresAutenticacao
{
    public class ConfiguracaoModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private ILogger<ConfiguracaoModel> Logger { get; }
        private UrlEncoder UrlEncoder { get; }
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        [BindProperty] 
        public InputModel Input { get; set; }

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ConfiguracaoModel(UserManager<ApplicationUser> userManager, ILogger<ConfiguracaoModel> logger, UrlEncoder urlEncoder)
        {
            UserManager = userManager;
            Logger = logger;
            UrlEncoder = urlEncoder;
        }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Código de verificação")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            await LoadSharedKeyAndQrCodeUriAsync(user);
            if (string.IsNullOrEmpty(SharedKey))
            {
                await UserManager.ResetAuthenticatorKeyAsync(user);
                await LoadSharedKeyAndQrCodeUriAsync(user);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            // Strip spaces and hypens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2FaTokenValid = await UserManager.VerifyTwoFactorTokenAsync(user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError("Input.Code", "O código de verificação é inválido.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            await UserManager.SetTwoFactorEnabledAsync(user, true);
            Logger.LogInformation("O usuário com o ID '{UserId}' ativou o 2FA com um aplicativo autenticador.", user.Id);

            var recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData["RecoveryCodes"] = recoveryCodes.ToArray();

            return RedirectToPage("./CodigoRecuperacao");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
            if (!string.IsNullOrEmpty(unformattedKey))
            {
                SharedKey = FormatKey(unformattedKey);
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;

            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(AuthenicatorUriFormat, UrlEncoder.Encode("BancoVirtualEstudantilWeb"), UrlEncoder.Encode(email), unformattedKey);
        }
    }
}