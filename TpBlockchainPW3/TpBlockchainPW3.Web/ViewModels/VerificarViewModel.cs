using System.ComponentModel.DataAnnotations;

namespace TpBlockchainPW3.Web.ViewModels;

public class VerificarViewModel
{
    [Required(ErrorMessage = "Seleccioná un archivo.")]
    [Display(Name = "Archivo a verificar")]
    public IFormFile? Archivo { get; set; } // IFormFile = archivo subido por HTTP
}