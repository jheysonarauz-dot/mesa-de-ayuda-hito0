# Sistema de Mesa de Ayuda

Aplicación web en **Blazor** (.NET 10) para registrar y dar seguimiento a solicitudes de soporte técnico.
Los tickets se guardan en **MySQL** (acceso a datos con **ADO.NET**), por lo que se conservan al cerrar y volver a ejecutar la aplicación.

## Integrantes
* Jheyson Araúz
* Jeremy Thorp

## Requisitos
- [.NET SDK 10.0](https://dotnet.microsoft.com/download) o superior
- MySQL 8.x (local o alojado). También funciona con MariaDB 10.5 o superior
- Un cliente para ejecutar el script: HeidiSQL, MySQL Workbench o la línea de comandos `mysql`
- Git

## Instalación y ejecución

### 1. Crear la base de datos con el script
El script `Database/mesa_ayuda.sql` crea la base `mesa_ayuda`, las tablas `proyectos` y `tickets`, y datos de muestra (ficticios). Se puede ejecutar más de una vez sin duplicar datos.

- **HeidiSQL / Workbench:** conectarse al servidor, abrir `Database/mesa_ayuda.sql` y ejecutarlo completo.
- **Línea de comandos:**
  ```
  mysql -u TU_USUARIO -p < Database/mesa_ayuda.sql
  ```

Para comprobar que se creó correctamente:
```sql
USE mesa_ayuda;
SELECT COUNT(*) FROM proyectos;   -- 4
SELECT COUNT(*) FROM tickets;     -- 6
```

### 2. Configurar la conexión a la base de datos

Cada integrante debe configurar la conexión a MySQL o MariaDB según su instalación local. El servidor, puerto, usuario y contraseña pueden ser diferentes entre los integrantes. Estos valores no deben estar fijados en el código ni compartirse mediante el repositorio.

Para configurar la conexión de forma segura, se utiliza **.NET User Secrets**, que permite almacenar las credenciales localmente, fuera del repositorio.

Desde la carpeta del proyecto que contiene `MesaAyuda.csproj`, cada integrante debe ejecutar:

```powershell
dotnet user-secrets set "ConnectionStrings:MesaAyuda" "Server=127.0.0.1;Port=PUERTO;Database=mesa_ayuda;User ID=USUARIO;Password=CONTRASEÑA;"
```

Cada integrante debe reemplazar los valores de ejemplo por los correspondientes a su instalación.

**Ejemplos:**
- MySQL en el puerto `3306`.
- MariaDB mediante XAMPP en el puerto `3307`.

También se puede utilizar la variable de entorno `ConnectionStrings__MesaAyuda` para proporcionar la cadena de conexión.

**Importante:** No subir contraseñas ni credenciales personales al repositorio de GitHub. Cada integrante configura sus propios datos localmente.

### 3. Ejecutar
```
git clone https://github.com/jheysonarauz-dot/mesa-de-ayuda-hito0.git
cd mesa-de-ayuda-hito0
dotnet run
```
Abrir en el navegador la dirección que indica la consola (por defecto `http://localhost:5181`).

Si la conexión no está bien configurada, la aplicación no se cierra: muestra un aviso de error en pantalla.

## Funciones disponibles
- **Registrar** tickets con título, descripción, proyecto y prioridad (Baja, Media o Alta). Cada ticket recibe un ID único y queda **Abierto**.
- **Listar** tickets con ID, título, proyecto, prioridad y estado, y **consultar** el detalle completo de cualquiera.
- **Editar** un ticket abierto sin duplicarlo ni afectar otros.
- **Cerrar** un ticket con una nota de resolución obligatoria. Un ticket cerrado no se puede cerrar otra vez ni editar.
- **Eliminar** con confirmación: cancelar conserva el ticket; confirmar elimina solo el seleccionado.
- **Información relacionada:** se muestra el nombre del proyecto, no solo su ID.
- **Filtrar** por estado y por proyecto, con aviso cuando no hay coincidencias y un botón para volver al listado completo. El filtro nunca modifica ni borra registros.
- **Cantidades** de total, abiertos y cerrados del conjunto completo (aunque haya un filtro activo), actualizadas tras cada operación.
- **Validación y avisos** que distinguen éxito, entrada inválida, ausencia de resultados y fallo de la base de datos.

## Organización del código
| Carpeta | Responsabilidad |
|---|---|
| `Components/` | Interfaz en Blazor: página principal (`Pages/Tickets.razor`) y formulario reutilizable (`Shared/`) |
| `Services/` | Lógica de la aplicación (`TicketServicio`) y el resultado de cada operación (`Resultado<T>`) |
| `Models/` | Modelo `Ticket` con sus reglas, `Proyecto`, `EstadoTicket` y `ConteoTickets` |
| `Data/` | Contrato `ITicketRepositorio` y su implementación con ADO.NET (`TicketRepositorioMySql`) |
| `Database/` | Script SQL de la base de datos |

La interfaz usa el servicio, el servicio usa el contrato del repositorio y solo el repositorio conoce MySQL.

## Seguridad
- Todas las consultas usan **parámetros** (`@titulo`, `@id`, etc.); los valores de entrada nunca se concatenan dentro del SQL.
- Las credenciales no están en el repositorio (user secrets o variable de entorno).
- Las reglas del ticket están protegidas en el modelo (constructor privado, propiedades con `private set`); la validación de la interfaz solo las acompaña.

## Cambios respecto del Hito 1
- La interfaz de consola se reemplazó por Blazor.
- Los tickets pasaron de una lista en memoria a MySQL con ADO.NET.
- `Proyecto` pasó de ser un texto a una tabla relacionada (`proyectos`), para mostrar información relacionada y filtrar.
- Se agregaron editar, eliminar con confirmación, filtro por proyecto y avisos de éxito, ausencia y fallo.
- Se conservaron las reglas del Hito 1: título y descripción obligatorios, prioridad permitida, cierre con nota y un ticket cerrado no se cierra dos veces.

## Limitaciones conocidas
- No hay usuarios ni autenticación.
- Solo existen los estados Abierto y Cerrado, y un ticket cerrado no se puede reabrir ni editar.
- No hay paginación: el listado muestra todos los tickets.
- Si dos personas modifican el mismo ticket a la vez, prevalece el último cambio guardado.

## Documentación anterior
- [Propuesta (Hito 0)](./hito-0.md)
- [Cambios y correcciones (Hito 1)](./hito-1.md)
