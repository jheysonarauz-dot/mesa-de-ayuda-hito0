namespace MesaAyuda.Models;

/// <summary>
/// Modelo del ticket. Protege sus propias reglas (encapsulamiento):
/// las propiedades solo se pueden modificar desde esta clase, y los datos
/// siempre pasan por la validacion antes de aceptarse.
/// </summary>
public sealed class Ticket
{
    public const int LongitudMaximaTitulo = 120;
    public const int LongitudMaximaDescripcion = 1000;
    public const int LongitudMaximaNota = 500;

    public static readonly IReadOnlyList<string> PrioridadesPermitidas = new[] { "Baja", "Media", "Alta" };

    public int Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int ProyectoId { get; private set; }
    public string ProyectoNombre { get; private set; } = string.Empty;
    public string Prioridad { get; private set; } = string.Empty;
    public EstadoTicket Estado { get; private set; } = EstadoTicket.Abierto;
    public DateTime FechaCreacion { get; private set; }
    public string? NotaResolucion { get; private set; }

    // Constructor privado: un ticket solo se obtiene con Crear (datos nuevos) o Reconstruir (datos de la base).
    private Ticket() { }

    /// <summary>Devuelve la lista de errores de los datos recibidos (vacia si son validos).</summary>
    public static List<string> Validar(string? titulo, string? descripcion, int proyectoId, string? prioridad)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(titulo))
            errores.Add("El título no puede estar vacío.");
        else if (titulo.Trim().Length > LongitudMaximaTitulo)
            errores.Add($"El título no puede superar {LongitudMaximaTitulo} caracteres.");

        if (string.IsNullOrWhiteSpace(descripcion))
            errores.Add("La descripción no puede estar vacía.");
        else if (descripcion.Trim().Length > LongitudMaximaDescripcion)
            errores.Add($"La descripción no puede superar {LongitudMaximaDescripcion} caracteres.");

        if (proyectoId <= 0)
            errores.Add("Debe seleccionar un proyecto.");

        if (NormalizarPrioridad(prioridad) is null)
            errores.Add($"La prioridad debe ser una de: {string.Join(", ", PrioridadesPermitidas)}.");

        return errores;
    }

    /// <summary>Crea un ticket nuevo: siempre queda Abierto y solo si los datos son validos.</summary>
    public static Ticket Crear(string? titulo, string? descripcion, int proyectoId, string? prioridad)
    {
        var errores = Validar(titulo, descripcion, proyectoId, prioridad);
        if (errores.Count > 0)
            throw new ArgumentException(string.Join(" ", errores));

        return new Ticket
        {
            Titulo = titulo!.Trim(),
            Descripcion = descripcion!.Trim(),
            ProyectoId = proyectoId,
            Prioridad = NormalizarPrioridad(prioridad)!,
            Estado = EstadoTicket.Abierto,
            FechaCreacion = DateTime.Now
        };
    }

    /// <summary>Reconstruye un ticket con los valores que ya estan guardados en la base de datos.</summary>
    public static Ticket Reconstruir(int id, string titulo, string descripcion, int proyectoId, string proyectoNombre,
        string prioridad, EstadoTicket estado, DateTime fechaCreacion, string? notaResolucion)
    {
        return new Ticket
        {
            Id = id,
            Titulo = titulo,
            Descripcion = descripcion,
            ProyectoId = proyectoId,
            ProyectoNombre = proyectoNombre,
            Prioridad = prioridad,
            Estado = estado,
            FechaCreacion = fechaCreacion,
            NotaResolucion = notaResolucion
        };
    }

    /// <summary>Modifica los datos editables. Un ticket cerrado no se puede editar.</summary>
    public void Editar(string? titulo, string? descripcion, int proyectoId, string? prioridad)
    {
        if (Estado == EstadoTicket.Cerrado)
            throw new InvalidOperationException("Un ticket cerrado no se puede editar.");

        var errores = Validar(titulo, descripcion, proyectoId, prioridad);
        if (errores.Count > 0)
            throw new ArgumentException(string.Join(" ", errores));

        Titulo = titulo!.Trim();
        Descripcion = descripcion!.Trim();
        ProyectoId = proyectoId;
        Prioridad = NormalizarPrioridad(prioridad)!;
    }

    /// <summary>Cierra el ticket. No se puede cerrar dos veces y exige una nota de resolucion.</summary>
    public void Cerrar(string? nota)
    {
        if (Estado == EstadoTicket.Cerrado)
            throw new InvalidOperationException("El ticket ya está cerrado.");

        if (string.IsNullOrWhiteSpace(nota))
            throw new ArgumentException("Debe indicar una nota de resolución para cerrar el ticket.");

        if (nota.Trim().Length > LongitudMaximaNota)
            throw new ArgumentException($"La nota de resolución no puede superar {LongitudMaximaNota} caracteres.");

        Estado = EstadoTicket.Cerrado;
        NotaResolucion = nota.Trim();
    }

    // Devuelve la prioridad con su escritura oficial ("alta" -> "Alta") o null si no esta permitida.
    private static string? NormalizarPrioridad(string? prioridad)
    {
        if (string.IsNullOrWhiteSpace(prioridad)) return null;
        return PrioridadesPermitidas.FirstOrDefault(p => p.Equals(prioridad.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
