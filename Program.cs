using MesaAyuda;

var repo = new TicketRepositorio();
EstadoTicket? filtro = null;

// Datos de muestra ficticios (sin personas ni solicitudes privadas)
repo.Registrar("Dependencia vulnerable", "NuGet Audit reporta un paquete con vulnerabilidad conocida.", "Proyecto Demo", "Alta");
repo.Registrar("Secreto expuesto", "Se detectó una clave de ejemplo en un archivo de configuración.", "Proyecto Demo", "Media");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== MESA DE AYUDA ===");
    Console.WriteLine($"Total: {repo.Total} | Abiertos: {repo.Abiertos} | Cerrados: {repo.Cerrados}");
    Console.WriteLine($"Filtro actual: {(filtro?.ToString() ?? "Todos")}");
    Console.WriteLine("1. Registrar ticket");
    Console.WriteLine("2. Listar tickets");
    Console.WriteLine("3. Consultar ticket");
    Console.WriteLine("4. Cerrar ticket");
    Console.WriteLine("5. Cambiar filtro (todos/abiertos/cerrados)");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": Registrar(); break;
        case "2": Listar(); break;
        case "3": Consultar(); break;
        case "4": Cerrar(); break;
        case "5": CambiarFiltro(); break;
        case "0": return;
        default: Console.WriteLine("Opción no válida."); break;
    }
}

void Registrar()
{
    Console.Write("Título: ");
    var titulo = Console.ReadLine();
    Console.Write("Descripción: ");
    var descripcion = Console.ReadLine();
    Console.Write("Proyecto afectado: ");
    var proyecto = Console.ReadLine();
    Console.Write($"Prioridad ({string.Join("/", Ticket.PrioridadesPermitidas)}): ");
    var prioridad = Console.ReadLine();

    var errores = Ticket.Validar(titulo, descripcion, proyecto, prioridad);
    if (errores.Count > 0)
    {
        Console.WriteLine("No se registró el ticket. Corrija lo siguiente:");
        errores.ForEach(e => Console.WriteLine($" - {e}"));
        return;
    }
    var t = repo.Registrar(titulo!, descripcion!, proyecto!, prioridad!);
    Console.WriteLine($"Ticket #{t.Id} registrado (Abierto).");
}

void Listar()
{
    var lista = repo.Listar(filtro);
    if (lista.Count == 0) { Console.WriteLine("No hay tickets para mostrar."); return; }
    Console.WriteLine("ID  | Estado   | Título");
    foreach (var t in lista) Console.WriteLine($"{t.Id,-3} | {t.Estado,-8} | {t.Titulo}");
}

Ticket? PedirTicket()
{
    Console.Write("ID del ticket: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("ID no válido."); return null; }
    var t = repo.ObtenerPorId(id);
    if (t == null) Console.WriteLine("No existe un ticket con ese ID.");
    return t;
}

void Consultar()
{
    var t = PedirTicket();
    if (t == null) return;
    Console.WriteLine($"ID: {t.Id}\nTítulo: {t.Titulo}\nDescripción: {t.Descripcion}\nProyecto: {t.Proyecto}\nPrioridad: {t.Prioridad}\nEstado: {t.Estado}\nCreado: {t.FechaCreacion:g}");
    if (t.NotaResolucion != null) Console.WriteLine($"Nota de resolución: {t.NotaResolucion}");
}

void Cerrar()
{
    var t = PedirTicket();
    if (t == null) return;
    Console.Write("Nota de resolución: ");
    var nota = Console.ReadLine();
    try { t.Cerrar(nota); Console.WriteLine($"Ticket #{t.Id} cerrado."); }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentException) { Console.WriteLine(ex.Message); }
}

void CambiarFiltro()
{
    Console.Write("1=Todos, 2=Abiertos, 3=Cerrados: ");
    filtro = Console.ReadLine()?.Trim() switch
    {
        "2" => EstadoTicket.Abierto,
        "3" => EstadoTicket.Cerrado,
        _ => null
    };
    Listar();
}

