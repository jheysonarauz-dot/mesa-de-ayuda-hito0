# Propuesta y Modelo Inicial (Hito 0) 

## 1. Situación y usuario
En el desarrollo de software actual, el registro, análisis y seguimiento de las solicitudes de soporte técnico y reporte de vulnerabilidades se manejan de forma fragmentada. Hoy en día, los desarrolladores y equipos de TI reciben reportes de fallos, solicitudes de auditoría y hallazgos de seguridad mediante canales informales como correo electrónico, chats internos o notas manuales. Esta falta de estandarización dificulta el seguimiento trazable de los problemas, impidiendo saber con certeza quién reportó un fallo, qué versión del código o paquete está afectada y en qué estado de resolución se encuentra cada solicitud.

El sistema será utilizado por desarrolladores independientes, líderes de equipo y administradores de seguridad. Estos usuarios necesitan registrar solicitudes de revisión, consultar el historial de hallazgos (como secretos expuestos o dependencias vulnerables con NuGet Audit) y verificar si un ticket ha sido atendido o resuelto mediante una actualización de versión o rotación de claves.

El sistema resolverá la desorganización centralizando los tickets de soporte técnico, asociando cada solicitud con un proyecto, una gravedad estimada y un estado transparente de resolución.

## 2. Acciones y resultados
El sistema permitirá gestionar el ciclo de vida de las solicitudes mediante cuatro acciones principales:

1. **Registrar Ticket:**
   - **Acción:** La persona usuaria completa un formulario ingresando el título, descripción, proyecto afectado y nivel de prioridad o gravedad.
   - **Resultado:** El sistema valida la información, asigna un identificador único, registra la fecha de creación y muestra una confirmación en pantalla junto con el detalle del ticket en estado "Nuevo".

2. **Listar Tickets:**
   - **Acción:** La persona usuaria accede al panel principal o aplica un filtro por estado (ej. "Nuevo", "En Proceso", "Resuelto").
   - **Resultado:** El sistema despliega una tabla interactiva con el resumen de las solicitudes, mostrando su ID, título, fecha, prioridad y estado actual.

3. **Consultar Ticket:**
   - **Acción:** La persona usuaria selecciona un ticket específico de la lista.
   - **Resultado:** El sistema presenta la vista detallada del ticket, incluyendo la descripción completa, el historial de cambios, los hallazgos o archivos asociados y las acciones disponibles.

4. **Cambiar Estado:**
   - **Acción:** Un usuario autorizado (ej. un desarrollador asignado) modifica el estado del ticket de "En Proceso" a "Resuelto" ingresando una nota de resolución.
   - **Resultado:** El sistema actualiza el registro, registra la traza de auditoría y muestra un mensaje informando que el estado fue cambiado exitosamente.

## 3. Clases iniciales
Para el modelo inicial en C#, se define la clase principal `Ticket` que encapsulated la información y reglas del negocio:

```csharp
public class Ticket
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Proyecto { get; set; }
    public string Prioridad { get; set; } // Alta, Media, Baja
    public string Estado { get; private set; } // Nuevo, EnProceso, Resuelto, Cerrado
    public DateTime FechaCreacion { get; set; }
    public string NotaResolucion { get; private set; }

    public Ticket()
    {
        Estado = "Nuevo";
        FechaCreacion = DateTime.Now;
    }

    public void CambiarEstado(string nuevoEstado, string nota)
    {
        if (Estado == "Cerrado")
            throw new InvalidOperationException("No se puede modificar un ticket cerrado.");

        if (nuevoEstado == "Resuelto" && string.IsNullOrWhiteSpace(nota))
            throw new ArgumentException("Debe proporcionar una nota de resolución para marcar el ticket como Resuelto.");

        Estado = nuevoEstado;
        NotaResolucion = nota;
    }
}
