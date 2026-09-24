using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("marcas_equipos")]
public class MarcaEquipo
{
    [Key]
    [Column("id_marca")]
    public int IdMarca { get; set; }

    [Required]
    [StringLength(50)]
    [Column("nombre_marca")]
    public string NombreMarca { get; set; } = string.Empty;

    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}