using System.Data.Common;
using MesaAyuda.Data;
using MesaAyuda.Models;

namespace MesaAyuda.Services;

/// <summary>
/// Logica de la aplicacion. Coordina el modelo (reglas) con el repositorio (datos)
/// y convierte los errores previsibles en un Resultado que la interfaz puede mostrar.
/// </summary>
public sealed class TicketServicio
{
    private readonly ITicketRepositorio _repositorio;
    private readonly ILogger<TicketServicio> _logger;

    public TicketServicio(ITicketRepositorio repositorio, ILogger<TicketServicio> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public Task<Resultado<IReadOnlyList<Ticket>>> ListarAsync(EstadoTicket? estado, int? proyectoId) =>
        EjecutarAsync("cargar los tickets", async () =>
            Resultado<IReadOnlyList<Ticket>>.Correcto(await _repositorio.ListarAsync(estado, proyectoId)));

    public Task<Resultado<ConteoTickets>> ContarAsync() =>
        EjecutarAsync("calcular las cantidades", async () =>
            Resultado<ConteoTickets>.Correcto(await _repositorio.ContarAsync()));

    public Task<Resultado<IReadOnlyList<Proyecto>>> ListarProyectosAsync() =>
        EjecutarAsync("cargar los proyectos", async () =>
            Resultado<IReadOnlyList<Proyecto>>.Correcto(await _repositorio.ListarProyectosAsync()));

    public Task<Resultado<Ticket>> ObtenerAsync(int id) =>
        EjecutarAsync("consultar el ticket", async () =>
        {
            var ticket = await _repositorio.ObtenerPorIdAsync(id);
            return ticket is null
                ? Resultado<Ticket>.NoEncontrado($"El ticket #{id} ya no existe.")
                : Resultado<Ticket>.Correcto(ticket);
        });

    public Task<Resultado<int>> RegistrarAsync(string? titulo, string? descripcion, int proyectoId, string? prioridad) =>
        EjecutarAsync("registrar el ticket", async () =>
        {
            var errores = Ticket.Validar(titulo, descripcion, proyectoId, prioridad);
            if (errores.Count == 0 && !await _repositorio.ExisteProyectoAsync(proyectoId))
                errores.Add("El proyecto seleccionado no existe.");
            if (errores.Count > 0)
                return Resultado<int>.Invalido(errores);

            var ticket = Ticket.Crear(titulo, descripcion, proyectoId, prioridad);
            int id = await _repositorio.InsertarAsync(ticket);
            return Resultado<int>.Correcto(id, $"Ticket #{id} registrado correctamente.");
        });

    public Task<Resultado<int>> EditarAsync(int id, string? titulo, string? descripcion, int proyectoId, string? prioridad) =>
        EjecutarAsync("editar el ticket", async () =>
        {
            var errores = Ticket.Validar(titulo, descripcion, proyectoId, prioridad);
            if (errores.Count == 0 && !await _repositorio.ExisteProyectoAsync(proyectoId))
                errores.Add("El proyecto seleccionado no existe.");
            if (errores.Count > 0)
                return Resultado<int>.Invalido(errores);

            var ticket = await _repositorio.ObtenerPorIdAsync(id);
            if (ticket is null)
                return Resultado<int>.NoEncontrado($"El ticket #{id} ya no existe.");

            try
            {
                ticket.Editar(titulo, descripcion, proyectoId, prioridad);   // el modelo aplica sus reglas
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                return Resultado<int>.Invalido(new[] { ex.Message });
            }

            bool actualizado = await _repositorio.ActualizarAsync(ticket);
            return actualizado
                ? Resultado<int>.Correcto(id, $"Ticket #{id} actualizado correctamente.")
                : Resultado<int>.NoEncontrado($"El ticket #{id} ya no existe.");
        });

    public Task<Resultado<int>> CerrarAsync(int id, string? nota) =>
        EjecutarAsync("cerrar el ticket", async () =>
        {
            var ticket = await _repositorio.ObtenerPorIdAsync(id);
            if (ticket is null)
                return Resultado<int>.NoEncontrado($"El ticket #{id} ya no existe.");

            try
            {
                ticket.Cerrar(nota);   // el modelo impide cerrar dos veces o sin nota
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                return Resultado<int>.Invalido(new[] { ex.Message });
            }

            bool actualizado = await _repositorio.ActualizarAsync(ticket);
            return actualizado
                ? Resultado<int>.Correcto(id, $"Ticket #{id} cerrado correctamente.")
                : Resultado<int>.NoEncontrado($"El ticket #{id} ya no existe.");
        });

    public Task<Resultado<int>> EliminarAsync(int id) =>
        EjecutarAsync("eliminar el ticket", async () =>
        {
            bool eliminado = await _repositorio.EliminarAsync(id);
            return eliminado
                ? Resultado<int>.Correcto(id, $"Ticket #{id} eliminado.")
                : Resultado<int>.NoEncontrado($"El ticket #{id} ya no existe.");
        });

    // Ejecuta una operacion y convierte los errores previsibles de acceso a datos en un Resultado de fallo.
    private async Task<Resultado<T>> EjecutarAsync<T>(string accion, Func<Task<Resultado<T>>> operacion)
    {
        try
        {
            return await operacion();
        }
        catch (Exception ex) when (ex is DbException or AccesoDatosException)
        {
            _logger.LogError(ex, "Error de acceso a datos al {Accion}", accion);
            return Resultado<T>.Fallo(
                $"Error al {accion}. Verifique que MySQL esté en ejecución y que la conexión esté bien configurada.");
        }
    }
}
