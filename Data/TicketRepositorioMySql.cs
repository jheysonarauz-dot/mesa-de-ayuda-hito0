using MesaAyuda.Models;
using MySqlConnector;

namespace MesaAyuda.Data;

/// <summary>
/// Acceso a datos con ADO.NET y MySQL. Todas las consultas usan parametros (@nombre):
/// los valores nunca se concatenan dentro del texto SQL.
/// </summary>
public sealed class TicketRepositorioMySql : ITicketRepositorio
{
    // Seleccion comun: une cada ticket con el nombre de su proyecto (informacion relacionada).
    private const string SeleccionTickets = @"
        SELECT t.id, t.titulo, t.descripcion, t.proyecto_id, p.nombre AS proyecto_nombre,
               t.prioridad, t.estado, t.fecha_creacion, t.nota_resolucion
        FROM tickets t
        INNER JOIN proyectos p ON p.id = t.proyecto_id";

    private readonly string? _cadenaConexion;

    public TicketRepositorioMySql(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("MesaAyuda");
    }

    // Crea la conexion y la abre. Cada metodo la usa dentro de "await using", que la cierra al terminar.
    private async Task<MySqlConnection> AbrirConexionAsync()
    {
        if (string.IsNullOrWhiteSpace(_cadenaConexion))
            throw new AccesoDatosException("No hay cadena de conexión configurada (ConnectionStrings:MesaAyuda).");

        var conexion = new MySqlConnection(_cadenaConexion);
        try
        {
            await conexion.OpenAsync();
            return conexion;
        }
        catch (ArgumentException ex)
        {
            // La cadena de conexion tiene un formato incorrecto.
            await conexion.DisposeAsync();
            throw new AccesoDatosException("La cadena de conexión no es válida.", ex);
        }
        catch
        {
            await conexion.DisposeAsync();
            throw;
        }
    }

    // Convierte la fila actual del lector en un objeto Ticket.
    private static Ticket LeerTicket(MySqlDataReader lector)
    {
        int posicionNota = lector.GetOrdinal("nota_resolucion");

        return Ticket.Reconstruir(
            id: lector.GetInt32(lector.GetOrdinal("id")),
            titulo: lector.GetString(lector.GetOrdinal("titulo")),
            descripcion: lector.GetString(lector.GetOrdinal("descripcion")),
            proyectoId: lector.GetInt32(lector.GetOrdinal("proyecto_id")),
            proyectoNombre: lector.GetString(lector.GetOrdinal("proyecto_nombre")),
            prioridad: lector.GetString(lector.GetOrdinal("prioridad")),
            estado: Enum.Parse<EstadoTicket>(lector.GetString(lector.GetOrdinal("estado"))),
            fechaCreacion: lector.GetDateTime(lector.GetOrdinal("fecha_creacion")),
            notaResolucion: lector.IsDBNull(posicionNota) ? null : lector.GetString(posicionNota));
    }

    public async Task<IReadOnlyList<Ticket>> ListarAsync(EstadoTicket? estado, int? proyectoId)
    {
        const string sql = SeleccionTickets + @"
            WHERE (@estado IS NULL OR t.estado = @estado)
              AND (@proyecto_id IS NULL OR t.proyecto_id = @proyecto_id)
            ORDER BY t.id DESC";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@estado", (object?)estado?.ToString() ?? DBNull.Value);
        comando.Parameters.AddWithValue("@proyecto_id", (object?)proyectoId ?? DBNull.Value);

        var tickets = new List<Ticket>();
        await using var lector = await comando.ExecuteReaderAsync();   // devuelve filas
        while (await lector.ReadAsync())
            tickets.Add(LeerTicket(lector));
        return tickets;
    }

    public async Task<Ticket?> ObtenerPorIdAsync(int id)
    {
        const string sql = SeleccionTickets + " WHERE t.id = @id";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        await using var lector = await comando.ExecuteReaderAsync();
        return await lector.ReadAsync() ? LeerTicket(lector) : null;
    }

    public async Task<int> InsertarAsync(Ticket ticket)
    {
        const string sql = @"
            INSERT INTO tickets (titulo, descripcion, proyecto_id, prioridad, estado, fecha_creacion, nota_resolucion)
            VALUES (@titulo, @descripcion, @proyecto_id, @prioridad, @estado, @fecha_creacion, @nota_resolucion)";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        AgregarParametrosDeTicket(comando, ticket);
        comando.Parameters.AddWithValue("@fecha_creacion", ticket.FechaCreacion);

        await comando.ExecuteNonQueryAsync();   // no devuelve filas: solo inserta
        return (int)comando.LastInsertedId;     // ID generado por AUTO_INCREMENT
    }

    public async Task<bool> ActualizarAsync(Ticket ticket)
    {
        const string sql = @"
            UPDATE tickets
            SET titulo = @titulo, descripcion = @descripcion, proyecto_id = @proyecto_id,
                prioridad = @prioridad, estado = @estado, nota_resolucion = @nota_resolucion
            WHERE id = @id";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        AgregarParametrosDeTicket(comando, ticket);
        comando.Parameters.AddWithValue("@id", ticket.Id);

        int filas = await comando.ExecuteNonQueryAsync();   // cantidad de filas afectadas
        return filas > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        const string sql = "DELETE FROM tickets WHERE id = @id";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        int filas = await comando.ExecuteNonQueryAsync();
        return filas > 0;   // false: el ticket ya no existia
    }

    public async Task<ConteoTickets> ContarAsync()
    {
        const string sql = @"
            SELECT COUNT(*) AS total,
                   COALESCE(SUM(estado = 'Abierto'), 0) AS abiertos,
                   COALESCE(SUM(estado = 'Cerrado'), 0) AS cerrados
            FROM tickets";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);

        await using var lector = await comando.ExecuteReaderAsync();
        await lector.ReadAsync();
        return new ConteoTickets(
            Total: Convert.ToInt32(lector.GetValue(0)),
            Abiertos: Convert.ToInt32(lector.GetValue(1)),
            Cerrados: Convert.ToInt32(lector.GetValue(2)));
    }

    public async Task<IReadOnlyList<Proyecto>> ListarProyectosAsync()
    {
        const string sql = "SELECT id, nombre FROM proyectos ORDER BY nombre";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);

        var proyectos = new List<Proyecto>();
        await using var lector = await comando.ExecuteReaderAsync();
        while (await lector.ReadAsync())
            proyectos.Add(new Proyecto(lector.GetInt32(0), lector.GetString(1)));
        return proyectos;
    }

    public async Task<bool> ExisteProyectoAsync(int proyectoId)
    {
        const string sql = "SELECT COUNT(*) FROM proyectos WHERE id = @id";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", proyectoId);

        object? resultado = await comando.ExecuteScalarAsync();   // devuelve un solo valor
        return Convert.ToInt32(resultado) > 0;
    }

    // Parametros que comparten el INSERT y el UPDATE.
    private static void AgregarParametrosDeTicket(MySqlCommand comando, Ticket ticket)
    {
        comando.Parameters.AddWithValue("@titulo", ticket.Titulo);
        comando.Parameters.AddWithValue("@descripcion", ticket.Descripcion);
        comando.Parameters.AddWithValue("@proyecto_id", ticket.ProyectoId);
        comando.Parameters.AddWithValue("@prioridad", ticket.Prioridad);
        comando.Parameters.AddWithValue("@estado", ticket.Estado.ToString());
        comando.Parameters.AddWithValue("@nota_resolucion", (object?)ticket.NotaResolucion ?? DBNull.Value);
    }
}
