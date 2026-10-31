namespace MesaAyuda;

public enum EstadoTicket { Abierto, Cerrado }

public class Ticket
{
    public static readonly string[] PrioridadesPermitidas = { "Baja", "Media", "Alta" };

    public int Id { get; }
    public string Titulo { get; }
    public string Descripcion { get; }
    public string Proyecto { get; }
    public string Prioridad { get; }
    public EstadoTicket Estado { get; private set; } = EstadoTicket.Abierto;
    public DateTime FechaCreacion { get; } = DateTime.Now;
    public string? NotaResolucion { get; private set; }

    private Ticket(int id, string titulo, string descripcion, string proyecto, string prioridad)
    {
        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        Proyecto = proyecto;
        Prioridad = prioridad;
    }

    // Validación en el modelo: devuelve la lista de errores (vacía si todo está bien).
    public static List<string> Validar(string? titulo, string? descripcion, string? proyecto, string? prioridad)
    {
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(titulo)) errores.Add("El título no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(descripcion)) errores.Add("La descripción no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(proyecto)) errores.Add("El proyecto afectado no puede estar vacío.");
        if (!PrioridadesPermitidas.Contains(prioridad?.Trim(), StringComparer.OrdinalIgnoreCase))
            errores.Add($"La prioridad debe ser una de: {string.Join(", ", PrioridadesPermitidas)}.");
        return errores;
    }

    public static Ticket Crear(int id, string titulo, string descripcion, string proyecto, string prioridad)
    {
        var errores = Validar(titulo, descripcion, proyecto, prioridad);
        if (errores.Count > 0) throw new ArgumentException(string.Join(" ", errores));
        var prioridadNormalizada = PrioridadesPermitidas.First(p => p.Equals(prioridad.Trim(), StringComparison.OrdinalIgnoreCase));
        return new Ticket(id, titulo.Trim(), descripcion.Trim(), proyecto.Trim(), prioridadNormalizada);
    }

    // Reglas de cambio de estado (heredadas del Hito 0):
    // un ticket cerrado no puede modificarse y el cierre exige una nota de resolución.
    public void Cerrar(string? nota)
    {
        if (Estado == EstadoTicket.Cerrado)
            throw new InvalidOperationException("El ticket ya está cerrado.");
        if (string.IsNullOrWhiteSpace(nota))
            throw new ArgumentException("Debe proporcionar una nota de resolución para cerrar el ticket.");
        Estado = EstadoTicket.Cerrado;
        NotaResolucion = nota.Trim();
    }
}
