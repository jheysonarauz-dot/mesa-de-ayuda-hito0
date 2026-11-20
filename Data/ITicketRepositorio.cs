using MesaAyuda.Models;

namespace MesaAyuda.Data;

/// <summary>
/// Contrato de acceso a datos. El servicio y la interfaz dependen de este contrato,
/// no de MySQL: asi cada capa usa a la otra sin conocer sus detalles internos.
/// </summary>
public interface ITicketRepositorio
{
    Task<IReadOnlyList<Ticket>> ListarAsync(EstadoTicket? estado, int? proyectoId);
    Task<Ticket?> ObtenerPorIdAsync(int id);
    Task<int> InsertarAsync(Ticket ticket);
    Task<bool> ActualizarAsync(Ticket ticket);
    Task<bool> EliminarAsync(int id);
    Task<ConteoTickets> ContarAsync();
    Task<IReadOnlyList<Proyecto>> ListarProyectosAsync();
    Task<bool> ExisteProyectoAsync(int proyectoId);
}
