using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteCreateUpdateDto
    {
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public int IdCarrera { get; set; }
        public string? FotoPerfilUrl { get; set; }

        // Datos del equipo portátil asociado (RF-05, RF-06)
        public string? SerialEquipo { get; set; }
        public int IdMarca { get; set; }
        public int IdTipo { get; set; }
        public string? FotoEquipoUrl { get; set; }
        public string? FotoAccesoriosUrl { get; set; }
    }
}