namespace SGE.Backend.DTOs.Equipos
{
    public class EquipoReadDto
    {
        public int IdEquipo { get; set; }
        public int IdEstudiante { get; set; }
        public string Serial { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string FotoEquipoUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<AccesorioReadDto> Accesorios { get; set; } = new();
    }

    public class AccesorioReadDto
    {
        public int IdAccesorio { get; set; }
        public string DescripcionAccesorio { get; set; } = string.Empty;
        public string? FotoAccesoriosUrl { get; set; }
    }
}