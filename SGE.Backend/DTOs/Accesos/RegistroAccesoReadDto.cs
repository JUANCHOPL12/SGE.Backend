namespace SGE.Backend.DTOs.Accesos
{
    public class RegistroAccesoReadDto
    {
        public int IdRegistro { get; set; }
        public string DocumentoEstudiante { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public string? SerialEquipo { get; set; }
        public string NombreGuardia { get; set; } = string.Empty;
        public int IdPorteria { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
    }
}