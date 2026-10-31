# Sistema de Mesa de Ayuda

Aplicación de consola en C# para registrar y dar seguimiento a solicitudes de soporte técnico (Hito 1).

## Integrantes
* Jheyson Araúz
* Jeremy Thorp

## Requisitos
- .NET SDK 10.0 o superior

## Ejecución
```
git clone https://github.com/jheysonarauz-dot/mesa-de-ayuda-hito0.git
cd mesa-de-ayuda-hito0
dotnet run
```

## Funciones disponibles
- Registrar tickets (título, descripción, proyecto y prioridad Baja/Media/Alta) con validación que indica qué corregir.
- Listar tickets (ID, título, estado) y filtrar por todos / abiertos / cerrados.
- Consultar el detalle completo de un ticket por ID.
- Cerrar un ticket con nota de resolución (un ticket cerrado no puede cerrarse otra vez).
- Contadores de total, abiertos y cerrados siempre visibles.

## Limitaciones conocidas
- Los datos viven solo en memoria: se pierden al cerrar la aplicación.
- Solo existe el cambio de estado Abierto → Cerrado (sin "En Proceso" ni reapertura).
- Interfaz de consola (sin Blazor), sin usuarios ni autenticación, sin historial de cambios.

## Documentación
- [Propuesta (Hito 0)](./hito-0.md)
- [Cambios y correcciones (Hito 1)](./hito-1.md)
