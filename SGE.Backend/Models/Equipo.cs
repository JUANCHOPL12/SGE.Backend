using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("equipos")]
public class Equipo
{
    [Key]
    [Column("id_equipo")]
    public int IdEquipo { get; set; }

    [Column("id_estudiante")]
    public int IdEstudiante { get; set; }

    [Column("id_tipo")]
    public int IdTipo { get; set; }

    [Column("id_marca")]
    public int IdMarca { get; set; }

    [Required]
    [StringLength(50)]
    [Column("serial")]
    public string Serial { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("foto_equipo_url")]
    public string FotoEquipoUrl { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Activo";

    [ForeignKey(nameof(IdEstudiante))]
    public Estudiante? Estudiante { get; set; }

    [ForeignKey(nameof(IdTipo))]
    public TipoEquipo? Tipo { get; set; }

    [ForeignKey(nameof(IdMarca))]
    public MarcaEquipo? Marca { get; set; }

    public ICollection<AccesorioEquipo> Accesorios { get; set; } = new List<AccesorioEquipo>();
    public ICollection<RegistroAcceso> RegistrosAccesos { get; set; } = new List<RegistroAcceso>();
}