using System.ComponentModel.DataAnnotations;

namespace TpBlockchainPW3.Web.Models
{
    public class VerificarPorCodigoModel
    {
        [Required(ErrorMessage = "Ingresá el código de verificación.")]
        [Display(Name = "Código de verificación")]
        public string Codigo { get; set; } = string.Empty;
    }
}
