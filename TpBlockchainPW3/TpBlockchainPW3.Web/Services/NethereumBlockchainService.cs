using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Options;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Services;

public class NethereumBlockchainService : IBlockchainService
{
    private readonly Web3 _web3;
    private readonly string _direccionContrato;
    private readonly ILogger<NethereumBlockchainService> _logger;

    public NethereumBlockchainService(
        IOptions<BlockchainOptions> opciones,
        ILogger<NethereumBlockchainService> logger)
    {
        var config = opciones.Value;

        // Si falta configuración, falla al arrancar con un mensaje claro
        if (string.IsNullOrWhiteSpace(config.RpcUrl) ||
            string.IsNullOrWhiteSpace(config.ContractAddress) ||
            string.IsNullOrWhiteSpace(config.PrivateKey))
        {
            throw new InvalidOperationException(
                "Falta configurar la sección Blockchain en User Secrets (ver README).");
        }

        var cuenta = new Account(config.PrivateKey, config.ChainId);   // firma con la clave privada
        _web3 = new Web3(cuenta, config.RpcUrl);                        // conexión a la red
        _web3.TransactionManager.UseLegacyAsDefault = true;             // formato más compatible con redes locales

        _direccionContrato = config.ContractAddress;
        _logger = logger;
    }

    public async Task<string> RegistrarAsync(string hashHex)
    {
        var funcion = new RegistrarFunction { Hash = HexABytes(hashHex) };

        // Escribir = TRANSACCIÓN: se firma, cuesta gas y deja comprobante
        var handler = _web3.Eth.GetContractTransactionHandler<RegistrarFunction>();
        var recibo = await handler.SendRequestAndWaitForReceiptAsync(_direccionContrato, funcion);

        if (recibo.Status.Value != 1)   // 1 = éxito, 0 = rechazada
        {
            throw new InvalidOperationException($"La transacción {recibo.TransactionHash} fue rechazada.");
        }

        _logger.LogInformation("Huella {Hash} registrada en la transacción {Tx}", hashHex, recibo.TransactionHash);
        return recibo.TransactionHash;
    }

    public async Task<ResultadoBlockchain> VerificarAsync(string hashHex)
    {
        var funcion = new VerificarFunction { Hash = HexABytes(hashHex) };

        // Leer = CONSULTA: no se firma, es gratis e instantánea
        var handler = _web3.Eth.GetContractQueryHandler<VerificarFunction>();
        var salida = await handler.QueryDeserializingToObjectAsync<VerificarOutput>(funcion, _direccionContrato);

        if (!salida.Existe)
        {
            return new ResultadoBlockchain(false, null, null);
        }

        var fecha = DateTimeOffset.FromUnixTimeSeconds((long)salida.Fecha).UtcDateTime;
        return new ResultadoBlockchain(true, salida.Emisor, fecha);
    }

    // La huella llega como texto hexadecimal (64 caracteres); el contrato la espera como bytes32
    private static byte[] HexABytes(string hashHex)
    {
        var bytes = Convert.FromHexString(hashHex);
        if (bytes.Length != 32)
        {
            throw new ArgumentException("La huella debe tener 64 caracteres hexadecimales.", nameof(hashHex));
        }
        return bytes;
    }
}