using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Data;
using TpBlockchainPW3.Web.Models;
using TpBlockchainPW3.Web.Services.Interfaces;

namespace TpBlockchainPW3.Web.Services;

public class DocumentoService : IDocumentoService
{
    private readonly ApplicationDbContext _context;
    private readonly IHashService _hashService;
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<DocumentoService> _logger;

    // Inyección de dependencias: .NET pasa estos objetos automáticamente
    // según lo registrado en Program.cs
    public DocumentoService(
        ApplicationDbContext context,
        IHashService hashService,
        IBlockchainService blockchainService,
        ILogger<DocumentoService> logger)
    {
        _context = context;
        _hashService = hashService;
        _blockchainService = blockchainService;
        _logger = logger;
    }

    public async Task<ResultadoRegistro> RegistrarAsync(NuevoDocumento datos)
    {
        // 1. Calcular la huella
        var hash = _hashService.CalcularHash(datos.Contenido);

        // 2. ¿Ya existe? (regla: no se registra dos veces la misma huella)
        var existente = await _context.Documentos
            .FirstOrDefaultAsync(d => d.HashSha256 == hash);

        if (existente != null)
        {
            return new ResultadoRegistro
            {
                Exito = false,
                Mensaje = $"Este archivo ya fue registrado con el código {existente.Codigo}.",
                Documento = existente
            };
        }

        // 3 y 4. Generar código y guardar como Pendiente
        var documento = new Documento
        {
            Codigo = await GenerarCodigoAsync(datos.Tipo),
            Titulo = datos.Titulo,
            Tipo = datos.Tipo,
            Contenido = datos.Contenido,
            NombreArchivo = datos.NombreArchivo,
            HashSha256 = hash,
            EmisorId = datos.EmisorId,
            NombreDestinatario = datos.NombreDestinatario,
            DniDestinatario = datos.DniDestinatario,
            Materia = datos.Materia,
            Nota = datos.Nota,
            Estado = EstadoDocumento.Pendiente
        };

        _context.Documentos.Add(documento);
        await _context.SaveChangesAsync();   // queda guardado aunque falle la blockchain

        // 5 y 6. Registrar en la blockchain y actualizar el estado
        try
        {
            documento.TxHash = await _blockchainService.RegistrarAsync(hash);
            documento.FechaRegistro = DateTime.UtcNow;
            documento.Estado = EstadoDocumento.Registrado;
        }
        catch (Exception ex)
        {
            // Si la red falla, el documento queda en Error (no se pierde)
            _logger.LogError(ex, "Error al registrar el documento {Codigo} en la blockchain", documento.Codigo);
            documento.Estado = EstadoDocumento.Error;
        }

        await _context.SaveChangesAsync();

        return new ResultadoRegistro
        {
            Exito = documento.Estado == EstadoDocumento.Registrado,
            Mensaje = documento.Estado == EstadoDocumento.Registrado
                ? "Documento registrado correctamente."
                : "El documento se guardó, pero no se pudo registrar en la blockchain.",
            Documento = documento
        };
    }

    public async Task<List<Documento>> ListarPorEmisorAsync(string emisorId)
    {
        return await _context.Documentos
            .Where(d => d.EmisorId == emisorId)          // solo los suyos
            .OrderByDescending(d => d.FechaCreacion)     // los más nuevos primero
            .ToListAsync();
    }

    public async Task<Documento?> ObtenerAsync(int id, string emisorId)
    {
        // Filtra también por emisor: nadie ve documentos de otro
        return await _context.Documentos
            .FirstOrDefaultAsync(d => d.Id == id && d.EmisorId == emisorId);
    }

    // Genera códigos tipo DOC-2026-0001 o CERT-2026-0001
    private async Task<string> GenerarCodigoAsync(TipoDocumento tipo)
    {
        var prefijo = tipo == TipoDocumento.Certificado ? "CERT" : "DOC";
        var inicio = $"{prefijo}-{DateTime.UtcNow.Year}-";

        var cantidad = await _context.Documentos
            .CountAsync(d => d.Codigo.StartsWith(inicio));

        return $"{inicio}{cantidad + 1:D4}";   // D4 = 4 dígitos con ceros: 0001
    }
}