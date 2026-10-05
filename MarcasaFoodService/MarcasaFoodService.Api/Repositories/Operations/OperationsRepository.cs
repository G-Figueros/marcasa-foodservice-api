using System.Data;
using Dapper;
using MarcasaFoodService.Api.Data;
using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Repositories.Operations;

// Toda la persistencia usa los SP existentes; los TVP conservan su orden y tipo SQL.
public sealed class OperationsRepository(ISqlConnectionFactory connections) : IOperationsRepository
{
    public Task<IReadOnlyList<ClienteDto>> ListarClientesAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        QueryAsync<ClienteDto>("dbo.usp_ListarClientes", new { TextoBusqueda = texto, SoloActivos = soloActivos }, ct);

    public Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        QueryAsync<ProductoDto>("dbo.usp_ListarProductos", new { TextoBusqueda = texto, SoloActivos = soloActivos }, ct);

    public Task<IReadOnlyList<InventarioDto>> ListarInventarioAsync(string? texto, bool soloActivos, CancellationToken ct) =>
        QueryAsync<InventarioDto>("dbo.usp_ListarInventario", new { TextoBusqueda = texto, SoloActivos = soloActivos }, ct);

    public Task<IReadOnlyList<PedidoResumenDto>> PendientesAutorizacionAsync(PedidoFiltro filtro, CancellationToken ct) =>
        QueryAsync<PedidoResumenDto>("dbo.usp_ListarPedidosPendientesAutorizacion", new
        {
            filtro.TextoBusqueda, filtro.ClienteId, filtro.FechaDesde, filtro.FechaHasta, filtro.IncluirDevueltos
        }, ct);

    public Task<IReadOnlyList<PedidoResumenDto>> AutorizadosRutaAsync(PedidoFiltro filtro, CancellationToken ct) =>
        QueryAsync<PedidoResumenDto>("dbo.usp_ListarPedidosAutorizadosRuta", FiltroRuta(filtro), ct);

    public Task<IReadOnlyList<PedidoResumenDto>> PendientesEntregaAsync(PedidoFiltro filtro, CancellationToken ct) =>
        QueryAsync<PedidoResumenDto>("dbo.usp_ListarPedidosPendientesEntrega", FiltroRuta(filtro), ct);

    public Task<IReadOnlyList<RutaPdfFilaDto>> DatosRutaAsync(PedidoFiltro filtro, CancellationToken ct) =>
        QueryAsync<RutaPdfFilaDto>("dbo.usp_ListarPedidosParaPdfRuta", FiltroRuta(filtro), ct);

    public async Task<PedidoDto?> ObtenerPedidoAsync(long pedidoId, CancellationToken ct)
    {
        var pedido = await SingleOrDefaultAsync<PedidoDto>("dbo.usp_ObtenerPedidoPorId", new { PedidoId = pedidoId }, ct);
        if (pedido is not null)
            pedido.Detalles = (await ObtenerDetalleAsync(pedidoId, ct)).ToList();
        return pedido;
    }

    public Task<IReadOnlyList<PedidoDetalleDto>> ObtenerDetalleAsync(long pedidoId, CancellationToken ct) =>
        QueryAsync<PedidoDetalleDto>("dbo.usp_ObtenerDetallePedido", new { PedidoId = pedidoId }, ct);

    public Task<CrearPedidoResultadoDto> CrearPedidoAsync(CrearPedidoRequest request, int usuarioId, CancellationToken ct)
    {
        var detalle = new DataTable();
        detalle.Columns.Add("ProductoId", typeof(int));
        detalle.Columns.Add("TipoPeso", typeof(string));
        detalle.Columns.Add("CantidadSolicitadaLbs", typeof(decimal));
        detalle.Columns.Add("CantidadSolicitadaBolsas", typeof(int));
        foreach (var item in request.Detalle)
            detalle.Rows.Add(item.ProductoId, item.TipoPeso,
                item.CantidadSolicitadaLbs is null ? DBNull.Value : item.CantidadSolicitadaLbs,
                item.CantidadSolicitadaBolsas is null ? DBNull.Value : item.CantidadSolicitadaBolsas);

        return SingleAsync<CrearPedidoResultadoDto>("dbo.usp_CrearPedido", new
        {
            request.ClienteId,
            UsuarioCreadorId = usuarioId,
            request.FechaEntregaProgramada,
            request.Observaciones,
            Detalle = detalle.AsTableValuedParameter("dbo.TVP_PedidoDetalleCrear")
        }, ct);
    }

    public Task<EstadoPedidoResultadoDto> AutorizarAsync(long pedidoId, AutorizarPedidoRequest request, int usuarioId, CancellationToken ct)
    {
        var detalle = new DataTable();
        detalle.Columns.Add("PedidoDetalleId", typeof(long));
        detalle.Columns.Add("MarcadoOK", typeof(bool));
        detalle.Columns.Add("Comentario", typeof(string));
        foreach (var item in request.DetalleAutorizacion)
            detalle.Rows.Add(item.PedidoDetalleId, item.MarcadoOK,
                item.Comentario is null ? DBNull.Value : item.Comentario);

        return SingleAsync<EstadoPedidoResultadoDto>("dbo.usp_AutorizarPedido", new
        {
            PedidoId = pedidoId,
            UsuarioAutorizaId = usuarioId,
            request.Comentario,
            DetalleAutorizacion = detalle.AsTableValuedParameter("dbo.TVP_AutorizacionPedidoDetalle")
        }, ct);
    }

    public Task<EstadoPedidoResultadoDto> DevolverAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct) =>
        SingleAsync<EstadoPedidoResultadoDto>("dbo.usp_DevolverPedidoCorreccion",
            new { PedidoId = pedidoId, UsuarioId = usuarioId, Comentario = comentario }, ct);

    public Task<EstadoPedidoResultadoDto> SolicitarCancelacionAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct) =>
        SingleAsync<EstadoPedidoResultadoDto>("dbo.usp_SolicitarCancelacionPedido",
            new { PedidoId = pedidoId, UsuarioId = usuarioId, Comentario = comentario }, ct);

    public Task<EstadoPedidoResultadoDto> AprobarCancelacionAsync(long pedidoId, string? comentario, int usuarioId, CancellationToken ct) =>
        SingleAsync<EstadoPedidoResultadoDto>("dbo.usp_AprobarCancelacionPedido",
            new { PedidoId = pedidoId, UsuarioId = usuarioId, Comentario = comentario }, ct);

    public Task<EstadoPedidoResultadoDto> ConfirmarEntregaAsync(long pedidoId, ConfirmarEntregaRequest request, int usuarioId, CancellationToken ct) =>
        SingleAsync<EstadoPedidoResultadoDto>("dbo.usp_ConfirmarEntregaPedido", new
        {
            PedidoId = pedidoId,
            UsuarioRepartidorId = usuarioId,
            request.Comentario,
            request.RutaArchivoEvidencia,
            request.NombreArchivoEvidencia,
            request.TipoContenidoEvidencia
        }, ct);

    private static object FiltroRuta(PedidoFiltro filtro) => new
    {
        filtro.TextoBusqueda, filtro.ClienteId, filtro.FechaDesde, filtro.FechaHasta
    };

    private async Task<IReadOnlyList<T>> QueryAsync<T>(string procedure, object args, CancellationToken ct)
    {
        await using var connection = connections.CreateConnection();
        var rows = await connection.QueryAsync<T>(new CommandDefinition(procedure, args,
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    private async Task<T> SingleAsync<T>(string procedure, object args, CancellationToken ct)
    {
        await using var connection = connections.CreateConnection();
        return await connection.QuerySingleAsync<T>(new CommandDefinition(procedure, args,
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    private async Task<T?> SingleOrDefaultAsync<T>(string procedure, object args, CancellationToken ct)
    {
        await using var connection = connections.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(procedure, args,
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }
}
