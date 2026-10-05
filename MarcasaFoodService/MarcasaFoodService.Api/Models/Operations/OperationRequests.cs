using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Operations;

public sealed class PedidoFiltro
{
    [StringLength(180)] public string? TextoBusqueda { get; set; }
    [Range(1, int.MaxValue)] public int? ClienteId { get; set; }
    [DataType(DataType.Date)] public DateTime? FechaDesde { get; set; }
    [DataType(DataType.Date)] public DateTime? FechaHasta { get; set; }
    public bool IncluirDevueltos { get; set; } = true;
}

public sealed class CrearPedidoRequest
{
    [Range(1, int.MaxValue)] public int ClienteId { get; set; }
    [DataType(DataType.Date)] public DateTime? FechaEntregaProgramada { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
    [Required, MinLength(1)] public List<CrearPedidoDetalleRequest> Detalle { get; set; } = [];
}

public sealed class CrearPedidoDetalleRequest : IValidatableObject
{
    [Range(1, int.MaxValue)] public int ProductoId { get; set; }
    [RegularExpression("^[NB]$")] public string TipoPeso { get; set; } = "";
    [Range(typeof(decimal), "0", "9999999999999999")] public decimal? CantidadSolicitadaLbs { get; set; }
    [Range(0, int.MaxValue)] public int? CantidadSolicitadaBolsas { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (CantidadSolicitadaLbs.GetValueOrDefault() <= 0 && CantidadSolicitadaBolsas.GetValueOrDefault() <= 0)
            yield return new ValidationResult("Indique libras o bolsas mayores que cero.");
    }
}

public sealed class AutorizarPedidoRequest
{
    [StringLength(500)] public string? Comentario { get; set; }
    [Required, MinLength(1)] public List<AutorizacionDetalleRequest> DetalleAutorizacion { get; set; } = [];
}

public sealed class AutorizacionDetalleRequest
{
    [Range(1, long.MaxValue)] public long PedidoDetalleId { get; set; }
    public bool MarcadoOK { get; set; }
    [StringLength(500)] public string? Comentario { get; set; }
}

public class ComentarioRequest
{
    [StringLength(500)] public string? Comentario { get; set; }
}

public sealed class ConfirmarEntregaRequest : ComentarioRequest
{
    [StringLength(500)] public string? RutaArchivoEvidencia { get; set; }
    [StringLength(250)] public string? NombreArchivoEvidencia { get; set; }
    [StringLength(100)] public string? TipoContenidoEvidencia { get; set; }
}
