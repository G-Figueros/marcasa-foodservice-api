namespace MarcasaFoodService.Api.Services.Operations;

// Códigos definidos en la tabla Permiso y emitidos como claims al iniciar sesión.
public static class Permissions
{
    public const string VerPedido = "VER_PEDIDO";
    public const string VerClientes = "VER_CLIENTES";
    public const string VerProductos = "VER_PRODUCTOS";
    public const string VerInventario = "VER_INVENTARIO";
    public const string CrearPedidos = "CREAR_PEDIDOS";
    public const string VerPendientes = "VER_PEDIDOS_PENDIENTES";
    public const string VerAutorizados = "VER_PEDIDOS_AUTORIZADOS";
    public const string VerPendientesEntrega = "VER_PEDIDOS_PENDIENTES_ENTREGA";
    public const string AutorizarPedidos = "AUTORIZAR_PEDIDOS";
    public const string DevolverPedidos = "DEVOLVER_PEDIDOS_CORRECCION";
    public const string SolicitarCancelacion = "SOLICITAR_CANCELACION_PEDIDOS";
    public const string AprobarCancelacion = "APROBAR_CANCELACION_PEDIDOS";
    public const string ExportarPdfRuta = "EXPORTAR_PDF_RUTA";
    public const string ConfirmarEntrega = "CONFIRMAR_ENTREGA";

    public static readonly string[] All =
    [
        VerClientes, VerProductos, VerInventario, CrearPedidos, VerPendientes,
        VerAutorizados, VerPendientesEntrega, AutorizarPedidos, DevolverPedidos,
        SolicitarCancelacion, AprobarCancelacion, ExportarPdfRuta, ConfirmarEntrega
    ];
}
