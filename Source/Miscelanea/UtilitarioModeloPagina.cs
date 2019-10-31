using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoVirtualEstudantilWeb.Miscelanea
{
    public static class UtilitarioModeloPagina
    {
        public static void DefinirEstadoMensagem(this PageModel modeloPagina, string mensagem)
        {
            modeloPagina.TempData["StatusMessage"] = mensagem;
        }
    }
}
