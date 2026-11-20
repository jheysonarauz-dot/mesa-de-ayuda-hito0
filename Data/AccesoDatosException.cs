namespace MesaAyuda.Data;

/// <summary>Error previsible de acceso a datos (por ejemplo, falta o es invalida la cadena de conexion).</summary>
public sealed class AccesoDatosException : Exception
{
    public AccesoDatosException(string mensaje, Exception? causa = null) : base(mensaje, causa) { }
}
