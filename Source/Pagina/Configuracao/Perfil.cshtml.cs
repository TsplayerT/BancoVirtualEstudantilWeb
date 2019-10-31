using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Miscelanea;
using BancoVirtualEstudantilWeb.Services.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoVirtualEstudantilWeb.Pagina.Configuracao
{
    public class PerfilModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private IMailManager EmailSender { get; }

        public PerfilModel(UserManager<ApplicationUser> userManager, IMailManager emailSender) 
        {
            UserManager = userManager;
            EmailSender = emailSender;
        }

        [BindProperty]
        [Required]
        public string UserName { get; set; }

        [BindProperty]
        [Required][EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Phone][Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;

            IsEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await UserManager.GetUserAsync(User);
            if (user == null) throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{UserManager.GetUserId(User)}'.");

            if (UserName != user.UserName)
            {
                var setUserNameResult = await UserManager.SetUserNameAsync(user, UserName);
                if (!setUserNameResult.Succeeded)
                {
                    TempData["StatusMessage"] = "Erro ao alterar o nome de usuário. " + string.Join(". ", setUserNameResult.Errors.Select(p => p.Description));
                    return RedirectToPage();
                    //throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Email != user.Email)
            {
                var setEmailResult = await UserManager.SetEmailAsync(user, Email);
                if (!setEmailResult.Succeeded)
                {
                    TempData["StatusMessage"] = $"Erro ao alterar o email: '{Email}' já está sendo usado em outra conta.";
                    return RedirectToPage();
                    //throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Ocorreu um erro inesperado ao definir o número de telefone do usuário com ID '{user.Id}'.");
                }
            }

            TempData["StatusMessage"] = "Seu perfil foi atualizado";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await UserManager.GetUserAsync(User);
            if (user == null) throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{ UserManager.GetUserId(User)}'.");

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.PegarUrlConfirmacaoEmail(user.Id, code, Request.Scheme);
            await EmailSender.EnviarEmailConfirmacaoAsync(user.Email, callbackUrl);

            TempData["StatusMessage"] = "E-mail de verificação enviado. Por favor verifique seu email.";

            return RedirectToPage();
        }
    }
}