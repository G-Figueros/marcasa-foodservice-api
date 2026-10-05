namespace MarcasaFoodService.Api.Models.Operations;

public sealed class ClienteDto
{
    public int ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string NombreComercial { get; set; } = "";
    public string? RazonSocial { get; set; }
    public string? NIT { get; set; }
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

public class ProductoDto
{
    public int ProductoId { get; set; }
    public string CodigoProducto { get; set; } = "";
    public string Nombre { get; set; } = "";
    public bool PermitePesoNeto { get; set; }
    public bool PermitePesoBruto { get; set; }
    public decimal? PesoNetoBolsaLbs { get; set; }
    public decimal? PesoBrutoBolsaLbs { get; set; }
    public bool SePesaAlDespachar { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

public sealed class InventarioDto
{
    public int ProductoId { get; set; }
    public string CodigoProducto { get; set; } = "";
    public string Nombre { get; set; } = "";
    public bool PermitePesoNeto { get; set; }
    public bool PermitePesoBruto { get; set; }
    public decimal? PesoNetoBolsaLbs { get; set; }
    public decimal? PesoBrutoBolsaLbs { get; set; }
    public bool SePesaAlDespachar { get; set; }
    public bool Activo { get; set; }
    public int InventarioId { get; set; }
    public decimal StockDisponibleLbs { get; set; }
    public decimal StockReservadoLbs { get; set; }
    public decimal StockTotalLbs { get; set; }
    public int CantidadBolsasDisponible { get; set; }
    public int CantidadBolsasReservada { get; set; }
    public int CantidadBolsasTotal { get; set; }
    public DateTime UltimaActualizacion { get; set; }
}
