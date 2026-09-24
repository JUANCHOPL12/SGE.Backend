using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGE.Backend.Models;

[Table("log_auditoria")]
public class LogAuditoria
{
    [Key]
    [Column("id_log")]
    public int IdLog { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(150)]
    [Column("accion_realizada")]
    public string AccionRealizada { get; set; } = string.Empty;

    [Column("fecha_hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; set; }
}