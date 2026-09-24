using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("porterias")]
public class Porteria
{
    [Key]
    [Column("id_porteria")]
    public int IdPorteria { get; set; }

    [Column("id_sede")]
    public int IdSede { get; set; }

    [Required]
    [StringLength(50)]
    [Column("nombre_porteria")]
    public string NombrePorteria { get; set; } = string.Empty;

    [ForeignKey(nameof(IdSede))]
    public Sede? Sede { get; set; }

    public ICollection<RegistroAcceso> RegistrosAccesos { get; set; } = new List<RegistroAcceso>();
}