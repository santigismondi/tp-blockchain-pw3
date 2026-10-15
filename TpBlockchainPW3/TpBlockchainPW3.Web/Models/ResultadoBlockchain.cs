namespace TpBlockchainPW3.Web.Models;

// Lo que responde la blockchain al preguntar por una huella
public record ResultadoBlockchain(bool Existe, string? Emisor, DateTime? Fecha);