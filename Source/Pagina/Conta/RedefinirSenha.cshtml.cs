using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BancoVirtualEstudantilWeb.Data;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class RedefinirSenhaModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        [BindProperty]
        public InputModel Input { get; set; }

        public RedefinirSenhaModel(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a Senha")]
            [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("Um código deve ser fornecido para redefinição de senha.");
            }

            Input = new InputModel
            {
                Code = code
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("./ConfirmacaoRedefinirSenha");
            }

            var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ConfirmacaoRedefinirSenha");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
