using System.ComponentModel.DataAnnotations;
using MesaAyuda.Models;

namespace MesaAyuda.Components.Shared;

/// <summary>
/// Datos del formulario. Sus atributos validan en la interfaz (mensajes inmediatos),
/// pero las reglas definitivas siguen estando en la clase Ticket.
/// </summary>
public sealed class TicketFormModel
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(Ticket.LongitudMaximaTitulo, ErrorMessage = "El título no puede superar {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(Ticket.LongitudMaximaDescripcion, ErrorMessage = "La descripción no puede superar {1} caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un proyecto.")]
    public int ProyectoId { get; set; }

    [Required(ErrorMessage = "Seleccione una prioridad.")]
    public string Prioridad { get; set; } = "Media";

    public static TicketFormModel Desde(Ticket ticket) => new()
    {
        Titulo = ticket.Titulo,
        Descripcion = ticket.Descripcion,
        ProyectoId = ticket.ProyectoId,
        Prioridad = ticket.Prioridad
    };
}
