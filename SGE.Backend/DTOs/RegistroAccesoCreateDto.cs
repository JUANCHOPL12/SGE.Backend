using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs
{
    public class RegistroAccesoCreateDto
    {
        [Required(ErrorMessage = "El identificador del estudiante es obligatorio.")]
        public int IdEstudiante { get; set; }

        // Puede ser nulo si el estudiante ingresa sin equipo
        public int? IdEquipo { get; set; }

        [Required(ErrorMessage = "El identificador de la portería es obligatorio.")]
        public int IdPorteria { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio (Entrada/Salida).")]
        [StringLength(20)]
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"
    }
}