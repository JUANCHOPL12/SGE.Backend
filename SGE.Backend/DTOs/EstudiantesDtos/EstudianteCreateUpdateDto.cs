using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteCreateUpdateDto
    {
        // DATOS OBLIGATORIOS DEL ESTUDIANTE (RF-05)
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        [Required(ErrorMessage = "La carrera es obligatoria.")]
        public int IdCarrera { get; set; }

        public string? FotoPerfilUrl { get; set; }

        // DATOS OPCIONALES DEL EQUIPO (RF-06) -> Al llevar el signo '?' ya NO son obligatorios
        public string? SerialEquipo { get; set; }
        public int? IdMarca { get; set; } // <--- Cambiado de int a int?
        public int? IdTipo { get; set; }  // <--- Cambiado de int a int?
        public string? FotoEquipoUrl { get; set; }
        public string? FotoAccesoriosUrl { get; set; }
    }
}