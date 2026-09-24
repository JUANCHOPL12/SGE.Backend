using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("carreras")]
public class Carrera
{
    [Key]
    [Column("id_carrera")]
    public int IdCarrera { get; set; }

    [Column("id_facultad")]
    public int IdFacultad { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre_carrera")]
    public string NombreCarrera { get; set; } = string.Empty;

    [ForeignKey(nameof(IdFacultad))]
    public Facultad? Facultad { get; set; }

    public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}