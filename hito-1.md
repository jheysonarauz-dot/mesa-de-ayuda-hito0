# Hito 1: cambios respecto del Hito 0

## Cambios respecto del Hito 0
- **Tipo de solución:** se implementó como aplicación de consola (el Hito 0 mencionaba C# y Blazor); el alcance del Hito 1 permite cualquiera de las dos.
- **Estados:** el Hito 0 proponía Nuevo, EnProceso, Resuelto y Cerrado como `string`. Se simplificó a `Abierto` y `Cerrado` con un `enum` (`EstadoTicket`), porque el Hito 1 solo exige un cambio de estado y el enum evita valores no permitidos.
- **Estado inicial:** "Nuevo" pasó a llamarse "Abierto".
- **Regla de cierre:** se conservó del Hito 0 la regla de que un ticket cerrado no se modifica y que cerrar exige una nota de resolución.
- **Inmutabilidad:** `Id`, `Titulo`, `Descripcion`, `Proyecto` y `Prioridad` pasaron de `set` público a solo lectura; los datos se validan al crear el ticket con `Ticket.Crear`.
- **Aplazado:** historial de cambios, traza de auditoría, hallazgos/archivos asociados y roles de usuario quedan fuera del alcance de esta iteración.

## Correcciones realizadas
- Se actualizó el proyecto de .NET 8 a .NET 10 (`net10.0`) para que compile con el SDK instalado.
- Se agregó `.vs/` al `.gitignore` para no subir archivos temporales de Visual Studio.

## Decisiones de diseño
- La validación vive en la clase `Ticket` (`Validar` y `Crear`): ningún ticket inválido puede existir.
- `Estado` tiene `private set`; solo cambia mediante `Cerrar(nota)`, que impide cerrar dos veces.
- `TicketRepositorio` concentra la lista en memoria, el filtro por estado y los contadores.
- Los datos de muestra son ficticios y no identifican personas.

## Contribución de cada integrante
- **Jheyson Araúz:** modelo `Ticket` (validaciones y regla de cierre), configuración del proyecto y documentación (`README.md` y `hito-1.md`).
- **Jeremy Thorp:** repositorio en memoria (`TicketRepositorio.cs`) y menú de consola (`Program.cs`).
