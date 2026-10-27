# Propuesta (Hito 0) - Prototipo de analizador de seguridad para código C#

Integrantes: Jeremy Thorp y Jheyson Arauz

## 1. Situación y usuario

Antes de publicar un programa, los desarrolladores suelen dejar sin querer malas prácticas de seguridad en el código: contraseñas escritas directamente, algoritmos de cifrado viejos o consultas SQL armadas sumando textos. Revisar esto a mano es lento y es fácil que se pase algo.

El sistema lo usaría un desarrollador o estudiante que quiere revisar un archivo `.cs` suyo antes de entregarlo, ver qué problemas tiene y llevar el control de cuáles ya corrigió.

El prototipo es un recorte de una idea más grande (plataforma con bóveda cifrada, rotación de claves y ofuscación). Nada de eso entra aquí.

## 2. Acciones y resultados

1. **Analizar archivo:**
   - **Acción:** el usuario escribe la ruta de un archivo `.cs`.
   - **Resultado:** el sistema valida que exista y sea `.cs`, lo analiza con tres reglas y muestra cuántos hallazgos encontró, todos en estado "Nuevo".

2. **Listar hallazgos:**
   - **Acción:** el usuario pide ver los hallazgos, con filtro opcional por estado o gravedad.
   - **Resultado:** una tabla con ID, regla, línea, gravedad y estado.

3. **Consultar hallazgo:**
   - **Acción:** el usuario elige un hallazgo por su ID.
   - **Resultado:** se muestra el detalle: descripción, código CWE, línea y recomendación.

4. **Cambiar estado:**
   - **Acción:** el usuario marca un hallazgo como Revisado, Corregido o Falso positivo.
   - **Resultado:** el estado se actualiza y se muestra confirmación. Para falso positivo pide una justificación.

## 3. Clase inicial

La clase principal es `Hallazgo` (un problema detectado en el código):

```csharp
// Jeremy Thorp - Clase Hallazgo del prototipo de analizador
public class Hallazgo
{
    public int Id { get; set; }
    public string Regla { get; set; }
    public string Cwe { get; set; }
    public int Linea { get; set; }
    public string Descripcion { get; set; }
    public string Gravedad { get; set; }       // Alta, Media, Baja
    public string Estado { get; private set; } // Nuevo, Revisado, Corregido, FalsoPositivo
    public string Justificacion { get; private set; }

    public Hallazgo() { Estado = "Nuevo"; }

    public void CambiarEstado(string nuevoEstado, string justificacion) { /* con las reglas de abajo */ }
}
```

Reglas de la clase:
- La línea debe ser mayor a 0 y la descripción no puede estar vacía.
- Todo hallazgo nace en estado Nuevo.
- Un hallazgo Corregido no puede volver a Nuevo ni a Revisado.
- Para marcar un falso positivo la justificación es obligatoria.

## 4. Cómo pensamos implementarlo

Para leer el código usamos Roslyn (el compilador de C# de Microsoft) solo como librería. Roslyn convierte el archivo en un árbol de nodos y la clase `CSharpSyntaxWalker` lo recorre y nos avisa cuando pasa por el tipo de nodo que nos interesa. No hacemos nada avanzado con Roslyn, la lógica de las reglas es la parte nuestra.

Clases y métodos:
- **ReglaSeguridad** (abstracta, hereda de `CSharpSyntaxWalker`): tiene nombre, CWE y gravedad, una lista de hallazgos y un método para agregar un hallazgo a partir de un nodo. Tiene tres clases hijas, cada una sobreescribe el método de visita que le corresponde:
  - ReglaCriptografiaInsegura (CWE-327): detecta llamadas a MD5 o SHA1.
  - ReglaSecretoExpuesto (CWE-798): detecta variables llamadas `password`, `apiKey`, `token` o `clave` con un texto fijo asignado.
  - ReglaSqlConcatenado (CWE-89): detecta sumas de textos donde uno empieza con SELECT, INSERT, UPDATE o DELETE. Es una heurística y puede dar falsos positivos, por eso existe ese estado.
- **AnalizadorArchivo**: guarda las reglas, y su método `Analizar(ruta)` valida el archivo, le pide el árbol a Roslyn, se lo pasa a cada regla y junta los hallazgos.
- **Reporte**: muestra, filtra por estado o gravedad y cuenta los hallazgos.
- **Program**: menú de consola que une todo.

Lo que aprenderíamos por nuestra cuenta: leer archivos en C# y lo básico de Roslyn (generar el árbol y heredar de `CSharpSyntaxWalker`).

Quedan fuera: cifrado, bóveda de versiones, base de datos, interfaz web, varios archivos a la vez, análisis de dependencias y ofuscación.
