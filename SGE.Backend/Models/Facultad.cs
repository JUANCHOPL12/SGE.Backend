using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("facultades")]
public class Facultad
{
    [Key]
    [Column("id_facultad")]
    public int IdFacultad { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre_facultad")]
    public string NombreFacultad { get; set; } = string.Empty;

    public ICollection<Carrera> Carreras { get; set; } = new List<Carrera>();
}