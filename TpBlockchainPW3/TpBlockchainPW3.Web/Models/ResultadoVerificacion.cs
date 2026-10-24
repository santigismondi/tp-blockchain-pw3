namespace TpBlockchainPW3.Web.Models;

// record: clase inmutable, ideal para "paquetes de datos" de solo lectura
public record ResultadoVerificacion(
    string HashSha256,         // la huella calculada del archivo subido
    bool Autentico,            // lo que dijo la BLOCKCHAIN (fuente de verdad)
    DateTime? FechaBlockchain, // cuándo se registró en la cadena
    string? EmisorBlockchain,  // la dirección (wallet) que lo registró
    Documento? Documento);     // datos de nuestra base, si existen (puede ser null)