using System.ComponentModel.DataAnnotations;

namespace SGE.Backend.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string NombreCompleto { get; set; } = string.Empty;

        // La contraseña es opcional: solo se enviará si el SuperAdmin desea cambiarla
        public string? Password { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string Estado { get; set; } = "Activo"; // "Activo" o "Inactivo"

        [Required(ErrorMessage = "El ID de Rol es obligatorio.")]
        public int IdRol { get; set; }
    }
}