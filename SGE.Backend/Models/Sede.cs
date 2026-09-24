using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("sedes")]
public class Sede
{
    [Key]
    [Column("id_sede")]
    public int IdSede { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre_sede")]
    public string NombreSede { get; set; } = string.Empty;

    [StringLength(150)]
    [Column("direccion")]
    public string? Direccion { get; set; }

    public ICollection<Porteria> Porterias { get; set; } = new List<Porteria>();
}