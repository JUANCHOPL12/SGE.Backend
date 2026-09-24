using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("estudiantes")]
public class Estudiante
{
    [Key]
    [Column("id_estudiante")]
    public int IdEstudiante { get; set; }

    [Required]
    [StringLength(20)]
    [Column("numero_documento")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [StringLength(100)]
    [Column("correo_electronico")]
    public string? CorreoElectronico { get; set; }

    [Column("id_carrera")]
    public int IdCarrera { get; set; }

    [StringLength(255)]
    [Column("foto_perfil_url")]
    public string? FotoPerfilUrl { get; set; }

    [StringLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Activo";

    [ForeignKey(nameof(IdCarrera))]
    public Carrera? Carrera { get; set; }

    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    public ICollection<RegistroAcceso> RegistrosAccesos { get; set; } = new List<RegistroAcceso>();
}