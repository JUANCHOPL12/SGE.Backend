using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteRegistroDto
    {
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(20)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [StringLength(100)]
        public string? CorreoElectronico { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una carrera.")]
        public int IdCarrera { get; set; }

        public string? FotoPerfilUrl { get; set; }
    }
}