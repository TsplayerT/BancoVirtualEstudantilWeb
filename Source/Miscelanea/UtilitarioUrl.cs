using Microsoft.AspNetCore.Mvc;

namespace BancoVirtualEstudantilWeb.Miscelanea
{
    public static class UtilitarioUrl
    {
        public static string PegarUrlLocal(this IUrlHelper urlHelper, string url)
        {
            return urlHelper.IsLocalUrl(url) ? url : urlHelper.Page("/Index");
        }

        public static string PegarUrlConfirmacaoEmail(this IUrlHelper urlHelper, string usuarioId, string codigo, string esquema)
        {
            return urlHelper.Page("/Conta/ConfirmacaoEmail", null, new {userId = usuarioId, code = codigo}, esquema);
        }

        public static string PegarUrlRedefinicaoSenha(this IUrlHelper urlHelper, string usuarioId, string codigo, string esquema)
        {
            return urlHelper.Page("/Conta/RedefinirSenha", null, new {userId = usuarioId, code = codigo}, esquema);
        }
    }
}
