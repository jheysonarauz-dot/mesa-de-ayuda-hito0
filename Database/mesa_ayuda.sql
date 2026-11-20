-- =============================================================================
-- Mesa de Ayuda (Hito 2) - Script de base de datos para MySQL
-- Crea la base, las tablas y los datos de muestra. Se puede ejecutar mas de
-- una vez: no borra ni duplica datos existentes.
-- Los datos de muestra son ficticios (no identifican personas).
-- =============================================================================

CREATE DATABASE IF NOT EXISTS mesa_ayuda
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE mesa_ayuda;

-- Proyectos afectados por los tickets (informacion relacionada).
CREATE TABLE IF NOT EXISTS proyectos (
    id     INT         NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(80) NOT NULL,
    CONSTRAINT pk_proyectos PRIMARY KEY (id),
    CONSTRAINT uq_proyectos_nombre UNIQUE (nombre)
) ENGINE = InnoDB;

-- Tickets de soporte. Las restricciones repiten las reglas del modelo C#.
CREATE TABLE IF NOT EXISTS tickets (
    id              INT           NOT NULL AUTO_INCREMENT,
    titulo          VARCHAR(120)  NOT NULL,
    descripcion     VARCHAR(1000) NOT NULL,
    proyecto_id     INT           NOT NULL,
    prioridad       VARCHAR(10)   NOT NULL,
    estado          VARCHAR(10)   NOT NULL DEFAULT 'Abierto',
    fecha_creacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    nota_resolucion VARCHAR(500)  NULL,
    CONSTRAINT pk_tickets PRIMARY KEY (id),
    CONSTRAINT fk_tickets_proyecto FOREIGN KEY (proyecto_id) REFERENCES proyectos (id),
    CONSTRAINT ck_tickets_prioridad CHECK (prioridad IN ('Baja', 'Media', 'Alta')),
    CONSTRAINT ck_tickets_estado CHECK (estado IN ('Abierto', 'Cerrado')),
    CONSTRAINT ck_tickets_nota CHECK (estado = 'Abierto' OR nota_resolucion IS NOT NULL),
    INDEX ix_tickets_estado (estado),
    INDEX ix_tickets_proyecto (proyecto_id)
) ENGINE = InnoDB;

-- Datos de muestra: proyectos.
INSERT IGNORE INTO proyectos (nombre) VALUES
    ('Portal Académico'),
    ('Sistema de Matrícula'),
    ('Biblioteca Digital'),
    ('Red del Laboratorio');

-- Datos de muestra: tickets (solo si la tabla esta vacia).
INSERT INTO tickets (titulo, descripcion, proyecto_id, prioridad, estado, fecha_creacion, nota_resolucion)
SELECT d.titulo, d.descripcion, p.id, d.prioridad, d.estado, d.fecha_creacion, d.nota_resolucion
FROM (
    SELECT 'No carga la página de calificaciones' AS titulo,
           'Al abrir la sección de calificaciones aparece una pantalla en blanco.' AS descripcion,
           'Portal Académico' AS proyecto, 'Alta' AS prioridad, 'Abierto' AS estado,
           '2026-11-02 09:15:00' AS fecha_creacion, NULL AS nota_resolucion
    UNION ALL
    SELECT 'Error al confirmar la matrícula',
           'El botón de confirmar no responde después de elegir las materias.',
           'Sistema de Matrícula', 'Alta', 'Abierto', '2026-11-03 14:40:00', NULL
    UNION ALL
    SELECT 'Actualizar el catálogo de libros',
           'Faltan los libros nuevos del segundo semestre en el buscador.',
           'Biblioteca Digital', 'Media', 'Abierto', '2026-11-04 11:05:00', NULL
    UNION ALL
    SELECT 'Conexión lenta en el laboratorio 2',
           'Los equipos tardan mucho en abrir las páginas durante las clases.',
           'Red del Laboratorio', 'Media', 'Cerrado', '2026-10-28 08:30:00',
           'Se reemplazó el cable del conmutador y la velocidad volvió a la normalidad.'
    UNION ALL
    SELECT 'Corregir el texto del menú principal',
           'El menú muestra una palabra con error de ortografía.',
           'Portal Académico', 'Baja', 'Cerrado', '2026-10-30 16:20:00',
           'Se corrigió el texto y se publicó el cambio.'
    UNION ALL
    SELECT 'Imprimir comprobante de matrícula',
           'No aparece la opción para descargar el comprobante en PDF.',
           'Sistema de Matrícula', 'Baja', 'Abierto', '2026-11-05 10:00:00', NULL
) AS d
INNER JOIN proyectos p ON p.nombre = d.proyecto
WHERE NOT EXISTS (SELECT 1 FROM tickets);
