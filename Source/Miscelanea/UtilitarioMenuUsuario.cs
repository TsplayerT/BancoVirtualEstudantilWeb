using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace BancoVirtualEstudantilWeb.Miscelanea
{
    public static class UtilitarioMenuUsuario
    {
        public static bool MenuAtivo(this IHtmlHelper htmlHelper, string urlItemMenu)
        {
            var viewContext = htmlHelper.ViewContext;
            var urlPaginaAtual = viewContext.ViewData["ActiveMenu"] as string ?? viewContext.HttpContext.Request.Path;
            return urlPaginaAtual.StartsWith(urlItemMenu, StringComparison.OrdinalIgnoreCase);
        }
    }
}
