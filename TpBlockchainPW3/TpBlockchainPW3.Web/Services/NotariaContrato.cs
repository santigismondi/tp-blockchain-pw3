using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace TpBlockchainPW3.Web.Services;

// Representa registrar(bytes32 hash) del contrato
[Function("registrar")]
public class RegistrarFunction : FunctionMessage
{
    [Parameter("bytes32", "hash", 1)]
    public byte[] Hash { get; set; } = [];
}

// Representa verificar(bytes32 hash) del contrato
[Function("verificar", typeof(VerificarOutput))]
public class VerificarFunction : FunctionMessage
{
    [Parameter("bytes32", "hash", 1)]
    public byte[] Hash { get; set; } = [];
}

// Lo que devuelve verificar: (bool existe, address emisor, uint256 fecha)
[FunctionOutput]
public class VerificarOutput : IFunctionOutputDTO
{
    [Parameter("bool", "existe", 1)]
    public bool Existe { get; set; }

    [Parameter("address", "emisor", 2)]
    public string Emisor { get; set; } = string.Empty;

    [Parameter("uint256", "fecha", 3)]
    public BigInteger Fecha { get; set; }   // segundos desde 1970
}