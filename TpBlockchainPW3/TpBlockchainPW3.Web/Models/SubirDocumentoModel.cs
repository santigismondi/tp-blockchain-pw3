using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TpBlockchainPW3.Web.Models
{
    public class SubirDocumentoModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        [Display(Name = "Título del documento")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes seleccionar un archivo.")]
        [Display(Name = "Archivo a registrar")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "El archivo no puede superar los 10 MB.")]
        public IFormFile Archivo { get; set; } = null!;

        [Required(ErrorMessage = "Elegí el tipo de documento.")]
        [Display(Name = "Tipo de Documento")]
        public TipoDocumento Tipo { get; set; } = TipoDocumento.Externo;

        [Display(Name = "Nombre del Destinatario")]
        [StringLength(150, ErrorMessage = "Puede tener hasta {1} caracteres.")]
        public string? NombreDestinatario { get; set; }

        [Display(Name = "DNI del Destinatario")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 números.")]
        public string? DniDestinatario { get; set; }

        [StringLength(150, ErrorMessage = "Puede tener hasta {1} caracteres.")]
        public string? Materia { get; set; }

        [Range(0, 10, ErrorMessage = "La nota debe estar entre 0 y 10.")]
        public decimal? Nota { get; set; }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && file.Length > _maxFileSize)
            {
                return new ValidationResult(ErrorMessage ?? $"El tamaño máximo permitido es de {_maxFileSize / (1024 * 1024)} MB.");
            }
            return ValidationResult.Success;
        }
    }
}