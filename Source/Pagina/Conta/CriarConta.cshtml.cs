using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Data.DataAnnotations;
using BancoVirtualEstudantilWeb.Miscelanea;
using BancoVirtualEstudantilWeb.Services.Mail;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class CriarContaModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private ILogger<ConectarModel> Logger { get; }
        private IMailManager EmailSender { get; }
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; private set; }

        public CriarContaModel(UserManager<ApplicationUser> userManager, ILogger<ConectarModel> logger, IMailManager emailSender)
        {
            UserManager = userManager;
            Logger = logger;
            EmailSender = emailSender;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Nome Completo")]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$)")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a Senha")]
            [Compare("Password", ErrorMessage = "As senhas não coincidem")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Acordo")]
            [IsTrueRequired(ErrorMessage = "Você deve concordar com os termos.")]
            public bool IsAgree { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.FullName, 
                    Email = Input.Email,
                    DataCriacao = DateTime.Now
                };
                var result = await UserManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    Logger.LogInformation(" usuário criou uma nova conta com senha.");

                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.PegarUrlConfirmacaoEmail(user.Id, code, Request.Scheme);
                    await EmailSender.EnviarEmailConfirmacaoAsync(Input.Email, callbackUrl);

                    return LocalRedirect(Url.PegarUrlLocal(returnUrl));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
