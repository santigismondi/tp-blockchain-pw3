namespace TpBlockchainPW3.Web.Models;

// Lo que hace falta para registrar un documento nuevo
public class NuevoDocumento
{
    public byte[] Contenido { get; set; } = [];
    public string NombreArchivo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public TipoDocumento Tipo { get; set; }
    public string EmisorId { get; set; } = string.Empty;

    // Solo para certificados
    public string? NombreDestinatario { get; set; }
    public string? DniDestinatario { get; set; }
    public string? Materia { get; set; }
    public decimal? Nota { get; set; }
    public string? Codigo { get; set; }
}