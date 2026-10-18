namespace SGE.Backend.DTOs.EstudiantesDtos
{
    public class EstudianteDetalleDto
    {
        public int IdEstudiante { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public string NombreCarrera { get; set; } = string.Empty;
        public string NombreFacultad { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Imágenes para verificación en portería (RF-07, RNF-04)
        public string? FotoPerfilUrl { get; set; }
        public string? FotoEquipoUrl { get; set; }
        public string? FotoAccesoriosUrl { get; set; }

        // Datos del equipo portátil
        public int? IdEquipo { get; set; }
        public string? SerialEquipo { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreTipo { get; set; }
        public bool TieneEquipoRegistrado { get; set; }

        // Estado de Control de Acceso (Para bloqueo de botones y temporizador en vivo)
        public bool EstaDentro { get; set; } // true = último movimiento fue Entrada
        public DateTime? UltimoIngreso { get; set; } // Fecha y hora exacta del ingreso
    }
}