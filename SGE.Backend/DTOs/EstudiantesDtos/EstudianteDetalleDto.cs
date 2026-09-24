namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteDetalleDto
    {
        public int IdEstudiante { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Imágenes para verificación en portería (RF-07, RNF-04)
        public string? FotoPerfilUrl { get; set; }
        public string? FotoEquipoUrl { get; set; }
        public string? FotoAccesoriosUrl { get; set; }

        // Datos del equipo portátil
        public string? SerialEquipo { get; set; }
        public bool TieneEquipoRegistrado { get; set; }
    }
}