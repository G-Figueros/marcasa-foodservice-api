using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Operations;

public sealed class GuardarClienteRequest
{
    [Required, StringLength(30, MinimumLength = 1)] public string CodigoCliente { get; set; } = "";
    [Required, StringLength(180, MinimumLength = 2)] public string NombreComercial { get; set; } = "";
    [StringLength(180)] public string? RazonSocial { get; set; }
    [StringLength(30)] public string? NIT { get; set; }
    [StringLength(120)] public string? Contacto { get; set; }
    [StringLength(25)] public string? Telefono { get; set; }
    [StringLength(300)] public string? Direccion { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
}

public sealed class EstadoClienteRequest
{
    public bool Activo { get; set; }
}

public sealed class GuardarProductoRequest
{
    [Required, StringLength(30, MinimumLength = 1)] public string CodigoProducto { get; set; } = "";
    [Required, StringLength(180, MinimumLength = 2)] public string Nombre { get; set; } = "";
    public bool PermitePesoNeto { get; set; }
    public bool PermitePesoBruto { get; set; }
    [Range(0.01, 99999999)] public decimal? PesoNetoBolsaLbs { get; set; }
    [Range(0.01, 99999999)] public decimal? PesoBrutoBolsaLbs { get; set; }
    public bool SePesaAlDespachar { get; set; }
}

public sealed class EstadoProductoRequest
{
    public bool Activo { get; set; }
}

public sealed class AjustarInventarioRequest
{
    [Range(0, 9999999999999999d)] public decimal StockDisponibleLbs { get; set; }
    [Range(0, int.MaxValue)] public int CantidadBolsasDisponible { get; set; }
    [Required, StringLength(300, MinimumLength = 5)] public string Observacion { get; set; } = "";
}
