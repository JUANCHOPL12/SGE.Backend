using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("tipos_equipos")]
public class TipoEquipo
{
    [Key]
    [Column("id_tipo")]
    public int IdTipo { get; set; }

    [Required]
    [StringLength(50)]
    [Column("nombre_tipo")]
    public string NombreTipo { get; set; } = string.Empty;

    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}