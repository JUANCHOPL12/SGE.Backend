namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteReadDto
    {
        public int IdEstudiante { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public int IdCarrera { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Imágenes (RNF-04)
        public string? FotoPerfilUrl { get; set; }
        public string? FotoEquipoUrl { get; set; }
        public string? FotoAccesoriosUrl { get; set; }

        // Datos del equipo (RF-05)
        public string? SerialEquipo { get; set; }
        public int? IdMarca { get; set; }
        public int? IdTipo { get; set; }
    }
}