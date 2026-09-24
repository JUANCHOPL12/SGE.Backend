using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("accesorios_equipo")]
public class AccesorioEquipo
{
    [Key]
    [Column("id_accesorio")]
    public int IdAccesorio { get; set; }

    [Column("id_equipo")]
    public int IdEquipo { get; set; }

    [Required]
    [StringLength(100)]
    [Column("descripcion_accesorio")]
    public string DescripcionAccesorio { get; set; } = string.Empty;

    [StringLength(255)]
    [Column("foto_accesorios_url")]
    public string? FotoAccesoriosUrl { get; set; }

    [ForeignKey(nameof(IdEquipo))]
    public Equipo? Equipo { get; set; }
}