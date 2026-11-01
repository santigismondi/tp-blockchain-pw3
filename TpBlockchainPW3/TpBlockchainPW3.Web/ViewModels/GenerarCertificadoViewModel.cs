using System.ComponentModel.DataAnnotations;

namespace TpBlockchainPW3.Web.ViewModels;

public class GenerarCertificadoViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 números.")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "La materia es obligatoria.")]
    [StringLength(100)]
    public string Materia { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nota es obligatoria.")]
    [Range(1, 10, ErrorMessage = "La nota debe estar entre 1 y 10.")]
    public int? Nota { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [FechaNoFutura]
    public DateTime? Fecha { get; set; }
}

public class FechaNoFuturaAttribute : ValidationAttribute
{
    public FechaNoFuturaAttribute() => ErrorMessage = "La fecha no puede ser futura.";

    public override bool IsValid(object? value) =>
        value is null || (value is DateTime d && d.Date <= DateTime.Today);
}