namespace MarcasaFoodService.Api.Models.Operations;

// Proyecciones de los resultados de los procedimientos almacenados de pedidos.
public class PedidoResumenDto
{
    public long PedidoId { get; set; }
    public int ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string NombreComercial { get; set; } = "";
    public string? RazonSocial { get; set; }
    public string? NIT { get; set; }
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int? UsuarioCreadorId { get; set; }
    public string? UsuarioCreador { get; set; }
    public string? NombreUsuarioCreador { get; set; }
    public int? EstadoPedidoId { get; set; }
    public string CodigoEstado { get; set; } = "";
    public string EstadoPedido { get; set; } = "";
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntregaProgramada { get; set; }
    public string? Observaciones { get; set; }
    public int TotalProductos { get; set; }
    public decimal TotalLibrasReservadas { get; set; }
    public int TotalBolsasReservadas { get; set; }
    public long? EntregaPedidoId { get; set; }
    public int? UsuarioRepartidorId { get; set; }
    public string? UsuarioRepartidor { get; set; }
    public DateTime? FechaSalidaRuta { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public bool? Entregado { get; set; }
    public int? UsuarioAutorizaId { get; set; }
    public string? UsuarioAutoriza { get; set; }
    public DateTime? FechaAutorizacion { get; set; }
    public string? ComentarioAutorizacion { get; set; }
}

public sealed class PedidoDto : PedidoResumenDto
{
    public long? AutorizacionPedidoId { get; set; }
    public string? ResultadoAutorizacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public List<PedidoDetalleDto> Detalles { get; set; } = [];
}

public sealed class PedidoDetalleDto
{
    public long PedidoDetalleId { get; set; }
    public long PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string CodigoProducto { get; set; } = "";
    public string Producto { get; set; } = "";
    public string TipoPeso { get; set; } = "";
    public string TipoPesoDescripcion { get; set; } = "";
    public bool PermitePesoNeto { get; set; }
    public bool PermitePesoBruto { get; set; }
    public decimal? PesoNetoBolsaLbs { get; set; }
    public decimal? PesoBrutoBolsaLbs { get; set; }
    public bool SePesaAlDespachar { get; set; }
    public decimal CantidadSolicitadaLbs { get; set; }
    public int CantidadSolicitadaBolsas { get; set; }
    public decimal CantidadReservadaLbs { get; set; }
    public int CantidadReservadaBolsas { get; set; }
    public decimal StockDisponibleLbs { get; set; }
    public decimal StockReservadoLbs { get; set; }
    public decimal StockTotalLbs { get; set; }
    public int CantidadBolsasDisponible { get; set; }
    public int CantidadBolsasReservada { get; set; }
    public int CantidadBolsasTotal { get; set; }
    public bool? UltimaRevisionOK { get; set; }
    public string? ComentarioRevision { get; set; }
    public DateTime? FechaUltimaRevision { get; set; }
}

public sealed class RutaPdfFilaDto
{
    public long PedidoId { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntregaProgramada { get; set; }
    public string CodigoEstado { get; set; } = "";
    public string EstadoPedido { get; set; } = "";
    public int ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string NombreComercial { get; set; } = "";
    public string? RazonSocial { get; set; }
    public string? NIT { get; set; }
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public long PedidoDetalleId { get; set; }
    public int ProductoId { get; set; }
    public string CodigoProducto { get; set; } = "";
    public string Producto { get; set; } = "";
    public string TipoPeso { get; set; } = "";
    public string TipoPesoDescripcion { get; set; } = "";
    public decimal CantidadSolicitadaLbs { get; set; }
    public int CantidadSolicitadaBolsas { get; set; }
    public decimal CantidadReservadaLbs { get; set; }
    public int CantidadReservadaBolsas { get; set; }
    public int? UsuarioAutorizaId { get; set; }
    public string? UsuarioAutoriza { get; set; }
    public DateTime? FechaAutorizacion { get; set; }
    public string? ComentarioAutorizacion { get; set; }
    public long? EntregaPedidoId { get; set; }
    public int? UsuarioRepartidorId { get; set; }
    public string? UsuarioRepartidor { get; set; }
    public DateTime? FechaSalidaRuta { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public bool? Entregado { get; set; }
    public string NombreRecibe { get; set; } = "";
    public string FirmaRecibe { get; set; } = "";
    public string ObservacionesEntrega { get; set; } = "";
}

public sealed class CrearPedidoResultadoDto
{
    public long PedidoId { get; set; }
    public int EstadoPedidoId { get; set; }
    public string Mensaje { get; set; } = "";
}

public sealed class EstadoPedidoResultadoDto
{
    public long PedidoId { get; set; }
    public string EstadoPedido { get; set; } = "";
    public string? Resultado { get; set; }
    public long? EntregaPedidoId { get; set; }
    public string Mensaje { get; set; } = "";
}
