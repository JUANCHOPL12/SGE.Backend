using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("registro_accesos")]
public class RegistroAcceso
{
    [Key]
    [Column("id_registro")]
    public int IdRegistro { get; set; }

    [Column("id_estudiante")]
    public int IdEstudiante { get; set; }

    [Column("id_equipo")]
    public int? IdEquipo { get; set; } // Opcional (NULL si ingresa sin equipo)

    [Column("id_usuario_guardia")]
    public int IdUsuarioGuardia { get; set; }

    [Column("id_porteria")]
    public int IdPorteria { get; set; }

    [Required]
    [StringLength(20)]
    [Column("tipo_movimiento")]
    public string TipoMovimiento { get; set; } = string.Empty;

    [Column("fecha_hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    [ForeignKey(nameof(IdEstudiante))]
    public Estudiante? Estudiante { get; set; }

    [ForeignKey(nameof(IdEquipo))]
    public Equipo? Equipo { get; set; }

    [ForeignKey(nameof(IdUsuarioGuardia))]
    public Usuario? Guardia { get; set; }

    [ForeignKey(nameof(IdPorteria))]
    public Porteria? Porteria { get; set; }
}