using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Miscelanea;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class ConectarModel : PageModel
    {
        private SignInManager<ApplicationUser> SignInManager { get; }
        private ILogger<ConectarModel> Logger { get; }
        [BindProperty]
        public InputModel Input { get; set; }
        private IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; private set; }
        [TempData] 
        private string ErrorMessage { get; set; }

        public ConectarModel(SignInManager<ApplicationUser> signInManager, ILogger<ConectarModel> logger)
        {
            SignInManager = signInManager;
            Logger = logger;
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await SignInManager.UserManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await SignInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, true);
                    if (result.Succeeded)
                    {
                        Logger.LogInformation("User logged in.");
                        return LocalRedirect(Url.PegarUrlLocal(returnUrl));
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./ConectarComAutenticacao", new { ReturnUrl = returnUrl, Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        Logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }

                    ModelState.AddModelError(string.Empty, "E-mail ou senha está incorreto.");
                    return Page();
                }

                ModelState.AddModelError(string.Empty, "User with this email not found");
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
