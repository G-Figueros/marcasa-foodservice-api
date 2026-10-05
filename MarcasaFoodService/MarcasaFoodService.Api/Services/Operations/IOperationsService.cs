using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Services.Operations;

public interface IOperationsService
{
    Task<IReadOnlyList<ClienteDto>> ClientesAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<ProductoDto>> ProductosAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<InventarioDto>> InventarioAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> PendientesAutorizacionAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> AutorizadosRutaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> PendientesEntregaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<RutaPdfFilaDto>> DatosRutaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<PedidoDto?> PedidoAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<PedidoDetalleDto>> DetalleAsync(long id, CancellationToken ct);
    Task<CrearPedidoResultadoDto> CrearAsync(CrearPedidoRequest request, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> AutorizarAsync(long id, AutorizarPedidoRequest request, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> DevolverAsync(long id, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> SolicitarCancelacionAsync(long id, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> AprobarCancelacionAsync(long id, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> ConfirmarEntregaAsync(long id, ConfirmarEntregaRequest request, int usuarioId, CancellationToken ct);
}
