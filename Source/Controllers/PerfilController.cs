using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BancoVirtualEstudantilWeb.Controllers
{
    public class PerfilController : Controller
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private SignInManager<ApplicationUser> SignInManager { get; }
        private ILogger<PerfilController> Logger { get; }
        private ApplicationUser CurrentUserPrivate { get; set; }

        public bool IsEmailConfirmed => UserManager.IsEmailConfirmedAsync(CurrentUser).Result;
        public bool IsHasPassword => UserManager.HasPasswordAsync(CurrentUser).Result;

        public PerfilController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<PerfilController> logger)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Logger = logger;
        }

        public ApplicationUser CurrentUser
        {
            get
            {
                if (CurrentUserPrivate == null)
                {
                    var user = UserManager.GetUserAsync(User);
                    if (user == null)
                    {
                        throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");
                    }

                    CurrentUserPrivate = user.Result;
                }
                return CurrentUserPrivate;
            }
        }

        public class ChangePasswordInput
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Senha atual")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nova senha")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a nova senha")]
            [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordInput model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var changePasswordResult = await UserManager.ChangePasswordAsync(CurrentUser, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            await SignInManager.SignInAsync(CurrentUser, false);
            Logger.LogInformation("O usuário alterou sua senha com sucesso.");

            return Ok("Sua senha foi mudada.");
        }

        public class SetPasswordInput
        {
            [Required]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nova senha")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a nova senha")]
            [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordInput model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addPasswordResult = await UserManager.AddPasswordAsync(CurrentUser, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            await SignInManager.SignInAsync(CurrentUser, false);
            Logger.LogInformation("Senha definida pelo usuário com sucesso.");

            return Ok("Your password has been set.");
        }
    }
}