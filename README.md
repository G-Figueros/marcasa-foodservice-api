# marcasa-foodservice-api
API REST en .NET 8 para SQL Server. La solución está organizada en controladores,
servicios (validación y reglas de entrada), repositorios (Dapper y procedimientos
almacenados), modelos y acceso a datos. Las transacciones de pedidos y stock se
ejecutan dentro de los procedimientos almacenados del directorio `BD`.

## Configuración local

1. Abrir `MarcasaFoodService/MarcasaFoodService.sln` y restaurar paquetes NuGet.
2. Revisar `MarcasaFoodService.Api/appsettings.Development.json`. La conexión
   comprobada es `GABRIELF8\SQLEXPRESS03`, con autenticación de Windows, y la base
   **`MarcasaFoodService_Dev`**. El nombre `MarcasFoodService_Dev` indicado en el
   mensaje no existe en esa instancia; los scripts también usan el primero.
3. Ejecutar desde `MarcasaFoodService`:

   ```powershell
   dotnet run --project MarcasaFoodService.Api --launch-profile https
   ```

4. Abrir `/swagger`. `POST /api/Auth/login` devuelve un JWT y los permisos; en
   Swagger, usar **Authorize** con ese token para consultar las demás rutas.

La clave JWT de desarrollo se genera en memoria al arrancar: reiniciar el API
invalida los tokens anteriores. En un entorno que no sea desarrollo, configurar
`ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Jwt__Issuer` y
`Jwt__Audience` mediante variables de entorno o un gestor de secretos. La clave
debe tener al menos 32 caracteres aleatorios. No guardar contraseñas en Git.

## Rutas principales

| Método | Ruta | Función |
| --- | --- | --- |
| POST | `/api/Auth/login` | Iniciar sesión y obtener permisos/JWT |
| GET | `/api/usuarios/me` | Identidad y permisos del token |
| GET | `/api/clientes`, `/api/productos`, `/api/inventario` | Catálogos y stock |
| GET | `/api/pedidos/pendientes-autorizacion` | Pendientes de revisión |
| GET | `/api/pedidos/autorizados-ruta` | Autorizados para ruta |
| GET | `/api/pedidos/{id}` y `/{id}/detalle` | Pedido y renglones |
| POST | `/api/pedidos` | Crear y reservar stock |
| POST | `/api/pedidos/{id}/autorizar` | Autorizar o devolver según revisión |
| POST | `/api/pedidos/{id}/devolver-correccion` | Devolver a corrección |
| POST | `/api/pedidos/{id}/solicitar-cancelacion` | Solicitar cancelación |
| POST | `/api/pedidos/{id}/aprobar-cancelacion` | Aprobar y liberar reserva |
| GET | `/api/entregas/pendientes` | Pedidos por entregar |
| GET | `/api/entregas/datos-ruta` | Datos para generar hoja/PDF de ruta |
| POST | `/api/entregas/{pedidoId}/confirmar` | Confirmar entrega y salida de stock |

Los listados aceptan `textoBusqueda` y `soloActivos`; los de pedidos y ruta
aceptan `textoBusqueda`, `clienteId`, `fechaDesde` y `fechaHasta`. Todas las
respuestas usan `{ success, message, data }`. La API toma el ID del usuario del
JWT y aplica políticas de permisos antes de invocar los SP.

`datos-ruta` entrega los datos estructurados, no un PDF binario. La confirmación
de entrega admite metadatos de evidencia ya almacenada; todavía no hay un
servicio de carga de archivos. La base actual no incluye SP de alta/edición de
clientes, productos y usuarios, por lo que esta API solo expone los flujos que
los SP disponibles permiten.
