using System.ComponentModel.DataAnnotations;

namespace BancoVirtualEstudantilWeb.Data.DataAnnotations
{
    public class IsTrueRequired : ValidationAttribute
    {
        public override bool IsValid(object value) => value is bool;
    }
}
