using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoVirtualEstudantilWeb.Pagina.Configuracao
{
    public class ContaModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}