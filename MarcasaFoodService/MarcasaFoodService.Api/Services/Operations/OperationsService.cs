using System.ComponentModel.DataAnnotations;
using MarcasaFoodService.Api.Models.Operations;
using MarcasaFoodService.Api.Repositories.Operations;

namespace MarcasaFoodService.Api.Services.Operations;

// Valida reglas de entrada; las transacciones y reglas de stock viven en SQL Server.
public sealed class OperationsService(IOperationsRepository repository) : IOperationsService
{
    public Task<IReadOnlyList<ClienteDto>> ClientesAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        repository.ListarClientesAsync(Limpiar(texto), soloActivos, ct);

    public Task<int> GuardarClienteAsync(int? clienteId, GuardarClienteRequest request, int actorId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoCliente) || string.IsNullOrWhiteSpace(request.NombreComercial))
            throw new ValidationException("El código y el nombre comercial son obligatorios.");
        return repository.GuardarClienteAsync(clienteId, request, actorId, ct);
    }

    public Task CambiarEstadoClienteAsync(int clienteId, bool activo, int actorId, CancellationToken ct) =>
        repository.CambiarEstadoClienteAsync(clienteId, activo, actorId, ct);

    public Task<IReadOnlyList<ProductoDto>> ProductosAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        repository.ListarProductosAsync(Limpiar(texto), soloActivos, ct);

    public Task<int> GuardarProductoAsync(int? productoId, GuardarProductoRequest request, int actorId, CancellationToken ct)
    {
        if (!request.PermitePesoNeto && !request.PermitePesoBruto)
            throw new ValidationException("Seleccioná al menos un tipo de peso.");
        if (string.IsNullOrWhiteSpace(request.CodigoProducto) || string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El código y el nombre son obligatorios.");
        return repository.GuardarProductoAsync(productoId, request, actorId, ct);
    }

    public Task CambiarEstadoProductoAsync(int productoId, bool activo, int actorId, CancellationToken ct) =>
        repository.CambiarEstadoProductoAsync(productoId, activo, actorId, ct);

    public Task AjustarInventarioAsync(int inventarioId, AjustarInventarioRequest request, int actorId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Observacion) || request.Observacion.Trim().Length < 5)
            throw new ValidationException("Indicá un motivo de al menos 5 caracteres para el ajuste.");
        return repository.AjustarInventarioAsync(inventarioId, request, actorId, ct);
    }

    public Task<IReadOnlyList<InventarioDto>> InventarioAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        repository.ListarInventarioAsync(Limpiar(texto), soloActivos, ct);

    public Task<IReadOnlyList<PedidoResumenDto>> PendientesAutorizacionAsync(PedidoFiltro filtro, CancellationToken ct)
    {
        ValidarFiltro(filtro);
        return repository.PendientesAutorizacionAsync(filtro, ct);
    }

    public Task<IReadOnlyList<PedidoResumenDto>> AutorizadosRutaAsync(PedidoFiltro filtro, CancellationToken ct)
    {
        ValidarFiltro(filtro);
        return repository.AutorizadosRutaAsync(filtro, ct);
    }

    public Task<IReadOnlyList<PedidoResumenDto>> PendientesEntregaAsync(PedidoFiltro filtro, CancellationToken ct)
    {
        ValidarFiltro(filtro);
        return repository.PendientesEntregaAsync(filtro, ct);
    }

    public Task<IReadOnlyList<RutaPdfFilaDto>> DatosRutaAsync(PedidoFiltro filtro, CancellationToken ct)
    {
        ValidarFiltro(filtro);
        return repository.DatosRutaAsync(filtro, ct);
    }

    public Task<PedidoDto?> PedidoAsync(long id, CancellationToken ct) => repository.ObtenerPedidoAsync(id, ct);
    public Task<IReadOnlyList<PedidoDetalleDto>> DetalleAsync(long id, CancellationToken ct) => repository.ObtenerDetalleAsync(id, ct);

    public Task<CrearPedidoResultadoDto> CrearAsync(CrearPedidoRequest request, int usuarioId, CancellationToken ct)
    {
        if (request.Detalle.Count > 200)
            throw new ValidationException("Un pedido admite hasta 200 renglones.");
        return repository.CrearPedidoAsync(request, usuarioId, ct);
    }

    public Task<EstadoPedidoResultadoDto> AutorizarAsync(long id, AutorizarPedidoRequest request, int usuarioId, CancellationToken ct)
    {
        if (request.DetalleAutorizacion.Count == 0 || request.DetalleAutorizacion.Any(x => !x.MarcadoOK))
            throw new ValidationException("Debés confirmar todos los productos antes de autorizar.");
        if (request.DetalleAutorizacion.Select(x => x.PedidoDetalleId).Distinct().Count() != request.DetalleAutorizacion.Count)
            throw new ValidationException("No repita detalles de autorización.");
        return repository.AutorizarAsync(id, request, usuarioId, ct);
    }

    public Task<EstadoPedidoResultadoDto> DevolverAsync(long id, string? comentario, int usuarioId, CancellationToken ct) =>
        repository.DevolverAsync(id, comentario, usuarioId, ct);

    public Task<EstadoPedidoResultadoDto> SolicitarCancelacionAsync(long id, string? comentario, int usuarioId, CancellationToken ct) =>
        repository.SolicitarCancelacionAsync(id, comentario, usuarioId, ct);

    public Task<EstadoPedidoResultadoDto> AprobarCancelacionAsync(long id, string? comentario, int usuarioId, CancellationToken ct) =>
        repository.AprobarCancelacionAsync(id, comentario, usuarioId, ct);

    public Task<EstadoPedidoResultadoDto> ConfirmarEntregaAsync(long id, ConfirmarEntregaRequest request, int usuarioId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.RutaArchivoEvidencia)
            && (string.IsNullOrWhiteSpace(request.NombreArchivoEvidencia)
                || string.IsNullOrWhiteSpace(request.TipoContenidoEvidencia)))
            throw new ValidationException("La evidencia requiere nombre y tipo de contenido.");
        return repository.ConfirmarEntregaAsync(id, request, usuarioId, ct);
    }

    private static void ValidarFiltro(PedidoFiltro filtro)
    {
        if (filtro.FechaDesde > filtro.FechaHasta)
            throw new ValidationException("La fecha inicial no puede superar la fecha final.");
        filtro.TextoBusqueda = Limpiar(filtro.TextoBusqueda);
    }

    private static string? Limpiar(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
