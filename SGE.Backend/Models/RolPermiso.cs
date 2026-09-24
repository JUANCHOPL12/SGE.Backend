using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("roles_permisos")]
public class RolPermiso
{
    [Column("id_rol")]
    public int IdRol { get; set; }

    [Column("id_permiso")]
    public int IdPermiso { get; set; }

    [ForeignKey(nameof(IdRol))]
    public Rol? Rol { get; set; }

    [ForeignKey(nameof(IdPermiso))]
    public Permiso? Permiso { get; set; }
}