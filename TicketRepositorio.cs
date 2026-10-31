// Jeremy Thorp - Guarda los tickets en memoria mientras el programa está abiertoB

namespace MesaAyuda;

// Almacén en memoria: conserva los tickets mientras la aplicación está en uso.
public class TicketRepositorio
{
    private readonly List<Ticket> _tickets = new();
    private int _siguienteId = 1;

    public Ticket Registrar(string titulo, string descripcion, string proyecto, string prioridad)
    {
        var ticket = Ticket.Crear(_siguienteId, titulo, descripcion, proyecto, prioridad);
        _siguienteId++;
        _tickets.Add(ticket);
        return ticket;
    }

    public Ticket? ObtenerPorId(int id) => _tickets.FirstOrDefault(t => t.Id == id);

    public IReadOnlyList<Ticket> Listar(EstadoTicket? filtro = null) =>
        _tickets.Where(t => filtro == null || t.Estado == filtro).ToList();

    public int Total => _tickets.Count;
    public int Abiertos => _tickets.Count(t => t.Estado == EstadoTicket.Abierto);
    public int Cerrados => _tickets.Count(t => t.Estado == EstadoTicket.Cerrado);
}

}