using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("permisos")]
public class Permiso
{
    [Key]
    [Column("id_permiso")]
    public int IdPermiso { get; set; }

    [Required]
    [StringLength(50)]
    [Column("nombre_permiso")]
    public string NombrePermiso { get; set; } = string.Empty;

    [StringLength(150)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    public ICollection<RolPermiso> RolesPermisos { get; set; } = new List<RolPermiso>();
}