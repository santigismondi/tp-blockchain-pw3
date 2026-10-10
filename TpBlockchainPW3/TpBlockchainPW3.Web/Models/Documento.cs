using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TpBlockchainPW3.Web.Models;

public class Documento
{
    public int Id { get; set; }   // clave primaria: EF la detecta por llamarse "Id"

    [Required, MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;   // ej. CERT-2026-0042

    [Required, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    public TipoDocumento Tipo { get; set; }

    // Solo para certificados (el "?" indica que pueden ser null)
    [MaxLength(150)]
    public string? NombreDestinatario { get; set; }

    [MaxLength(10)]
    public string? DniDestinatario { get; set; }

    [MaxLength(150)]
    public string? Materia { get; set; }

    public decimal? Nota { get; set; }

    // Archivo y huella
    [Required, MaxLength(260)]
    public string RutaArchivo { get; set; } = string.Empty;

    [Required, MaxLength(64)]   // SHA-256 en hexadecimal = 64 caracteres
    public string HashSha256 { get; set; } = string.Empty;

    // Datos del registro en blockchain (se completan después)
    [MaxLength(66)]             // "0x" + 64 caracteres hexadecimales
    public string? TxHash { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public EstadoDocumento Estado { get; set; } = EstadoDocumento.Pendiente;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relación con el usuario de Identity que lo creó
    [Required]
    public string EmisorId { get; set; } = string.Empty;   // clave foránea
    public IdentityUser? Emisor { get; set; }               // propiedad de navegación
}