using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoVirtualEstudantilWeb.Pagina.Conta
{
    public class DesconectarModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}