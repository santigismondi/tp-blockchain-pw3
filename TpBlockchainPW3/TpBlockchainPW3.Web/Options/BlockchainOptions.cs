namespace TpBlockchainPW3.Web.Options;

// Datos de conexión a la blockchain.
// Se leen de la configuración (appsettings.json + User Secrets), nunca del código.
public class BlockchainOptions
{
    public const string Seccion = "Blockchain";   // nombre de la sección en la configuración

    public string RpcUrl { get; set; } = string.Empty;           // dónde está la red
    public long ChainId { get; set; }                             // identificador de la red
    public string ContractAddress { get; set; } = string.Empty;  // dónde está el contrato
    public string PrivateKey { get; set; } = string.Empty;       // con qué cuenta firmamos
}