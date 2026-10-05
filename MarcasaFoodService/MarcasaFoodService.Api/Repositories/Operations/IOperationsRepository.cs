using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Repositories.Operations;

public interface IOperationsRepository
{
    Task<IReadOnlyList<ClienteDto>> ListarClientesAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<InventarioDto>> ListarInventarioAsync(string? texto, bool soloActivos, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> PendientesAutorizacionAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> AutorizadosRutaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<PedidoResumenDto>> PendientesEntregaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<IReadOnlyList<RutaPdfFilaDto>> DatosRutaAsync(PedidoFiltro filtro, CancellationToken ct);
    Task<PedidoDto?> ObtenerPedidoAsync(long pedidoId, CancellationToken ct);
    Task<IReadOnlyList<PedidoDetalleDto>> ObtenerDetalleAsync(long pedidoId, CancellationToken ct);
    Task<CrearPedidoResultadoDto> CrearPedidoAsync(CrearPedidoRequest request, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> AutorizarAsync(long pedidoId, AutorizarPedidoRequest request, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> DevolverAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> SolicitarCancelacionAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> AprobarCancelacionAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct);
    Task<EstadoPedidoResultadoDto> ConfirmarEntregaAsync(long pedidoId, ConfirmarEntregaRequest request, int usuarioId, CancellationToken ct);
}
