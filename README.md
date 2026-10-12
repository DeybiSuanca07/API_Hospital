# API_Hospital

API REST para la gestión de órdenes de servicios hospitalarios. Las órdenes se crean en estado **Pendiente**, y un proceso en segundo plano (Worker Service) las procesa de forma asíncrona y las cambia a **Procesada**.

Construida con **.NET 8**, **Clean Architecture**, **CQRS**, **Repository**, **Unit of Work** y **SQL Server** (Entity Framework Core 8).

---

## Requisitos

| Herramienta | Versión |
|---|---|
| .NET SDK | 8.0 o superior (para abrir `API_Hospital.slnx` desde la consola se necesita el SDK 9.0.200 o superior) |

---

## Pasos de ejecución

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/DeybiSuanca07/API_Hospital.git
   cd API_Hospital
   ```

2. **Configurar la cadena de conexión** a tu SQL Server (ver [Cadenas de conexión](#cadenas-de-conexión)).

3. **Crear la base de datos** aplicando las migraciones o ejecutando el script DDL (ver [Migraciones](#migraciones)).

4. **Ejecutar la API**
   - **Visual Studio:** abrir `API_Hospital.slnx`, dejar `API_Hospital` como proyecto de inicio y presionar `F5`.
   - **Consola:**
     ```bash
     dotnet run --project API_Hospital/API_Hospital.csproj --launch-profile https
     ```

5. **Probar** en Swagger: `https://localhost:7107/swagger`

Al iniciar, la consola muestra `Service Worker de órdenes iniciado`, lo que confirma que el procesamiento en segundo plano está activo.

---

## Cadenas de conexión

La API usa una única cadena de conexión llamada **`DefaultConnection`**, ubicada en `API_Hospital/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=Hospital;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Ejemplos según el tipo de instalación:

| Escenario | Cadena de conexión |
|---|---|
| Autenticación de Windows | `Server=localhost;Database=Hospital;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| Usuario y contraseña de SQL | `Server=localhost;Database=Hospital;User ID=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| Instancia con nombre | `Server=localhost\SQLEXPRESS;Database=Hospital;Trusted_Connection=True;TrustServerCertificate=True` |
| LocalDB | `Server=(localdb)\MSSQLLocalDB;Database=Hospital;Trusted_Connection=True` |

---

## Migraciones

Las migraciones ya están incluidas en el repositorio (`Hospital.Infrastructure/Migrations`). Al aplicarlas, **se crea la base de datos** si no existe y luego la tabla `Orders`. No hace falta crear nada a mano en SQL Server.

El usuario de la cadena de conexión necesita permisos para crear bases de datos (rol `dbcreator`), o la base `Hospital` debe existir previamente.

### Opción 1: Visual Studio (Consola del Administrador de Paquetes)

`Herramientas` → `Administrador de paquetes NuGet` → `Consola del Administrador de paquetes`:

```powershell
Update-Database -Project Hospital.Infrastructure -StartupProject API_Hospital
```

### Opción 2: Consola (.NET CLI)

```bash
dotnet tool install --global dotnet-ef --version 8.*
dotnet ef database update --project Hospital.Infrastructure --startup-project API_Hospital
```

### Opción 3: Script DDL (sin Entity Framework)

Ejecutar [`docs/ScriptBD.sql`](docs/ScriptBD.sql) en SQL Server Management Studio. Crea desde cero la base de datos `Hospital`, la tabla `Orders` y la tabla `__EFMigrationsHistory`.

El script registra las migraciones como aplicadas, así que después se puede ejecutar `Update-Database` sin conflictos. Basta con usar **una sola** de las tres opciones.

---

## Endpoints

| Método | Ruta | Descripción | Respuesta |
|---|---|---|---|
| `POST` | `/api/Orders` | Crea una orden (queda Pendiente) y retorna la orden creada | `201 Created` / `400` |
| `GET` | `/api/Orders/{id}` | Obtiene una orden por Id | `200` / `404` |
| `GET` | `/api/Orders?page=1&pageSize=10&patientId=&status=` | Lista órdenes paginadas, con filtros opcionales | `200` / `400` |

**Paginación:** `page` (por defecto `1`) y `pageSize` (por defecto `10`, máximo `100`). La respuesta incluye `items`, `page`, `pageSize`, `totalCount`, `totalPages`, `hasPreviousPage` y `hasNextPage`.

La documentación completa de la API (OpenAPI 3.0) está en [`docs/swagger.json`](docs/swagger.json).

---

## Logging

El logging usa **Serilog** y escribe en **consola** y en **archivo**.

### Ruta del archivo

```
API_Hospital/logs/hospital-AAAAMMDD.log
```

- La ruta es relativa al directorio de la API (`API_Hospital/`), tanto al ejecutar desde Visual Studio como con `dotnet run`.
- Se crea **un archivo por día** (por ejemplo `hospital-20261012.log`) y se conservan los **últimos 30 días**.
- La carpeta `logs/` se crea sola la primera vez que se ejecuta la API. Los logs de ejecución están excluidos del repositorio en `.gitignore`.
- La ruta, la retención y los niveles se configuran en la sección `Serilog` de `API_Hospital/appsettings.json`.

### Eventos registrados

| Evento | Nivel | Ejemplo |
|---|---|---|
| Creación de órdenes | `INF` | `Orden 8 creada en estado Pendiente. Paciente: 12345, Servicio: LAB-001, Prioridad: Urgente` |
| Inicio de procesamiento | `INF` | `Orden 8 (Urgente) en procesamiento` |
| Cambio de estado | `INF` | `Orden 8 cambió de estado: Pendiente -> Procesada` |
| Peticiones HTTP | `INF` | `HTTP POST /api/Orders responded 201 in 45.2 ms` |
| Errores de validación / no encontrado (400, 404, 409) | `WRN` | `POST /api/Orders con 400. ValidationException: La prioridad debe ser Normal o Urgente.` |
| Errores no controlados (500), con stack trace | `ERR` | `Error no controlado en GET /api/Orders/5` |
| Fallas del Worker | `ERR` | `Error procesando órdenes` |

Para ver el ciclo de vida completo de una orden, basta con buscar su identificador en el archivo (por ejemplo, `Orden 8`).

El nombre del paciente no se registra en los logs por tratarse de un dato personal.

### Evidencia de logs

En [`docs/logs/`](docs/logs/) se incluye un archivo de log generado en una ejecución real. Muestra la creación de órdenes, su procesamiento por el Worker con los cambios de estado, y el registro de errores.