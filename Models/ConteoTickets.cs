namespace MesaAyuda.Models;

/// <summary>Cantidades del conjunto completo de tickets (no dependen del filtro activo).</summary>
public readonly record struct ConteoTickets(int Total, int Abiertos, int Cerrados);
