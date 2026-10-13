using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs.Equipos
{
    public class CrearEquipoDto
    {
        [Required(ErrorMessage = "El ID del estudiante es obligatorio.")]
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "El ID del tipo de equipo es obligatorio.")]
        public int IdTipo { get; set; }

        [Required(ErrorMessage = "El ID de la marca es obligatorio.")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "El número de serial es obligatorio.")]
        [StringLength(50, ErrorMessage = "El serial no puede superar los 50 caracteres.")]
        public string Serial { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL de la foto del equipo es obligatoria.")]
        public string FotoEquipoUrl { get; set; } = string.Empty;

        // Campos opcionales para asociar accesorios en el mismo registro (RF-06)
        public string? DescripcionAccesorio { get; set; }
        public string? FotoAccesoriosUrl { get; set; }
    }
}