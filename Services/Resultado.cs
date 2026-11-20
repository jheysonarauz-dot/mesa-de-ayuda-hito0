namespace MesaAyuda.Services;

public enum TipoResultado
{
    Exito,          // la operacion se completo
    Invalido,       // la entrada no cumple las reglas: se indica que corregir
    NoEncontrado,   // el ticket (o dato) ya no existe
    Fallo           // error de ejecucion (por ejemplo, la base no responde)
}

/// <summary>Resultado de una operacion: distingue exito, entrada invalida, ausencia y fallo.</summary>
public sealed class Resultado<T>
{
    public TipoResultado Tipo { get; }
    public string Mensaje { get; }
    public IReadOnlyList<string> Errores { get; }
    public T? Valor { get; }
    public bool EsExito => Tipo == TipoResultado.Exito;

    private Resultado(TipoResultado tipo, string mensaje, T? valor = default, IReadOnlyList<string>? errores = null)
    {
        Tipo = tipo;
        Mensaje = mensaje;
        Valor = valor;
        Errores = errores ?? Array.Empty<string>();
    }

    public static Resultado<T> Correcto(T valor, string mensaje = "") =>
        new(TipoResultado.Exito, mensaje, valor);

    public static Resultado<T> Invalido(IReadOnlyList<string> errores) =>
        new(TipoResultado.Invalido, "Revise los datos ingresados.", default, errores);

    public static Resultado<T> NoEncontrado(string mensaje) =>
        new(TipoResultado.NoEncontrado, mensaje);

    public static Resultado<T> Fallo(string mensaje) =>
        new(TipoResultado.Fallo, mensaje);
}
