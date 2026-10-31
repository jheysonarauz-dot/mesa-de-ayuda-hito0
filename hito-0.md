# Propuesta y Modelo Inicial (Hito 0) - Sistema de Mesa de Ayuda

## 1. Situación y usuario
En muchas oficinas y áreas de TI, las solicitudes de soporte técnico (por ejemplo, un equipo que no enciende o una falla de conexión a internet) llegan por canales informales como mensajes, llamadas o correos. Sin un registro central es difícil saber qué solicitudes están pendientes, cuáles son más urgentes y cuáles ya se atendieron.

El sistema lo usarían las personas que reportan un problema y el personal de soporte que lo atiende. Necesitan registrar solicitudes, consultar su estado y saber si ya fueron resueltas.

El sistema resuelve la desorganización centralizando las solicitudes como tickets, y cada uno tiene un proyecto afectado, una prioridad y un estado claro de resolución.

## 2. Acciones y resultados
El sistema permitirá gestionar el ciclo de vida de las solicitudes mediante cuatro acciones principales:

1. **Registrar Ticket:**
   * **Acción:** La persona usuaria completa un formulario ingresando el título, descripción, proyecto afectado y nivel de prioridad.
   * **Resultado:** El sistema valida la información, asigna un identificador único, registra la fecha de creación y muestra una confirmación en pantalla junto con el detalle del ticket en estado "Nuevo".

2. **Listar Tickets:**
   * **Acción:** La persona usuaria accede al panel principal o aplica un filtro por estado (ej. "Nuevo", "En Proceso", "Resuelto").
   * **Resultado:** El sistema despliega una tabla con el resumen de las solicitudes, mostrando su ID, título, fecha, prioridad y estado actual.

3. **Consultar Ticket:**
   * **Acción:** La persona usuaria selecciona un ticket específico de la lista.
   * **Resultado:** El sistema presenta la vista detallada del ticket, incluyendo la descripción completa, el proyecto, la prioridad, el estado y las acciones disponibles.

4. **Cambiar Estado:**
   * **Acción:** Una persona de soporte modifica el estado del ticket de "En Proceso" a "Resuelto" ingresando una nota de resolución.
   * **Resultado:** El sistema actualiza el registro y muestra un mensaje informando que el estado fue cambiado exitosamente.

## 3. Clases iniciales
Para el modelo inicial en C#, se define la clase principal `Ticket`, que encapsula la información y las reglas del negocio:

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
```
