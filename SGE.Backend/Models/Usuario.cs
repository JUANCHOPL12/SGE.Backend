using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("usuarios")]
public class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre_completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("id_rol")]
    public int IdRol { get; set; }

    [StringLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Activo";

    [ForeignKey(nameof(IdRol))]
    public Rol? Rol { get; set; }

    public ICollection<LogAuditoria> LogsAuditoria { get; set; } = new List<LogAuditoria>();
    public ICollection<RegistroAcceso> RegistrosAccesosGuardia { get; set; } = new List<RegistroAcceso>();
}