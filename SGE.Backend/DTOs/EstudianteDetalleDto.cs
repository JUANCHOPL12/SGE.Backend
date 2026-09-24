namespace SGE.Backend.DTOs
{
    public class EstudianteDetalleDto
    {
        public int IdEstudiante { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Facultad { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Lista de equipos registrados a nombre del estudiante
        public List<EquipoDetalleDto> Equipos { get; set; } = new();
    }

    public class EquipoDetalleDto
    {
        public int IdEquipo { get; set; }
        public string Serial { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string FotoEquipoUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<string> Accesorios { get; set; } = new();
    }
}