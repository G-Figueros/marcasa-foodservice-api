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

5. Para una base existente, ejecutar una vez `BD/LÓGICA/usuarios_admin_v2.sql`.
   Agrega la versión de credenciales y los procedimientos de administración.
   El script es repetible y ya se aplicó a `MarcasaFoodService_Dev` local.

La clave JWT de desarrollo se genera en memoria al arrancar: reiniciar el API
invalida los tokens anteriores. En un entorno que no sea desarrollo, configurar
`ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Jwt__Issuer` y
`Jwt__Audience` mediante variables de entorno o un gestor de secretos. La clave
debe tener al menos 32 caracteres aleatorios. No guardar contraseñas en Git.

## Rutas principales

| Método | Ruta | Función |
| --- | --- | --- |
| POST | `/api/Auth/login` | Iniciar sesión y obtener permisos/JWT |
| POST | `/api/Auth/change-password` | Cambiar contraseña propia; exige la actual |
| GET | `/api/usuarios/me` | Identidad y permisos del token |
| GET | `/api/usuarios`, `/api/usuarios/roles` | Listar cuentas y roles (Gerente) |
| POST, PUT | `/api/usuarios`, `/api/usuarios/{id}` | Crear y editar cuentas (Gerente) |
| PATCH | `/api/usuarios/{id}/estado` | Activar o desactivar (Gerente) |
| POST | `/api/usuarios/{id}/reiniciar-password` | Restablecer contraseña temporal (Gerente) |
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
servicio de carga de archivos. Los productos y el inventario disponible se
administran mediante los SP de `BD/LÓGICA/catalogo_cuenta_v4.sql`.

Gerente y Supervisora pueden crear, editar y activar o desactivar clientes con
`POST /api/clientes`, `PUT /api/clientes/{id}` y
`PATCH /api/clientes/{id}/estado`. Aplicar `BD/LÓGICA/clientes_admin_v5.sql`
antes de ejecutar esta versión. `GET /api/clientes?soloActivos=false` incluye
clientes inactivos; `dbo.usp_CrearPedido` rechaza pedidos nuevos para ellos.

## Cuenta, productos e inventario

Aplicar `BD/LÓGICA/catalogo_cuenta_v4.sql` en cada base existente antes de
iniciar esta versión del API. Ya está aplicado en `MarcasaFoodService_Dev` local.
`GET/PUT /api/cuenta` permite consultar y editar los datos personales sin
cambiar rol ni nombre de usuario. `POST/PUT /api/productos` y
`PATCH /api/productos/{id}/estado` requieren Gerente o Supervisora con
`EDITAR_PRODUCTOS`. El alta crea inventario en cero; desactivar conserva el
producto y evita usarlo en nuevos pedidos. `PUT /api/inventario/{id}` requiere
`AGREGAR_STOCK` y ajusta solo el disponible, con un motivo registrado en
`MovimientoInventario`. La prueba reversible es
`BD/PRUEBAS/verificar_catalogo_v4.sql`.

Las cuentas nuevas y reiniciadas usan `Temporal2026*` y deben cambiarla al
ingresar. Las contraseñas nuevas se guardan con PBKDF2-SHA256; el login sigue
admitiendo el hash anterior para las cuentas existentes. Cada petición validada
comprueba si la cuenta sigue activa y si cambió su versión de credenciales; al
desactivar, cambiar rol o reiniciar contraseña, los JWT anteriores dejan de servir.

## Administración de usuarios

Ejecutar `BD/LÓGICA/usuarios_admin_v2.sql` una vez en una base existente. El script ya se aplicó a la base local `MarcasaFoodService_Dev`. El Gerente puede usar `GET /api/usuarios`, `GET /api/usuarios/roles`, `POST /api/usuarios`, `PUT /api/usuarios/{id}`, `PATCH /api/usuarios/{id}/estado` y `POST /api/usuarios/{id}/reiniciar-password`.

Las cuentas nuevas o reiniciadas usan `Temporal2026*` y deben cambiarla mediante `POST /api/Auth/change-password` antes de acceder a las demás rutas. Las nuevas contraseñas se guardan con PBKDF2-SHA256; las cuentas anteriores siguen siendo compatibles. Desactivar, cambiar rol o reiniciar contraseña invalida los JWT anteriores.

## Roles y permisos

Ejecutar `BD/LÓGICA/roles_admin_v3.sql` en bases existentes. El script se aplicó a `MarcasaFoodService_Dev` y es repetible. El Gerente administra roles mediante `GET /api/roles`, `GET /api/roles/permisos`, `POST /api/roles`, `PUT /api/roles/{id}` y `PATCH /api/roles/{id}/estado`. Los cambios de permisos invalidan las sesiones del rol afectado; un rol inactivo impide iniciar sesión. El rol Gerente conserva siempre la administración de roles y usuarios.

La autorización de pedidos exige todos los productos confirmados tanto en la API como en `dbo.usp_AutorizarPedido`.
