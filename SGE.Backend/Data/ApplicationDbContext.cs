using Microsoft.EntityFrameworkCore;
using SGE.Backend.Models;

namespace SGE.Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Facultad> Facultades { get; set; }
    public DbSet<Carrera> Carreras { get; set; }
    public DbSet<Estudiante> Estudiantes { get; set; }
    public DbSet<MarcaEquipo> MarcasEquipos { get; set; }
    public DbSet<TipoEquipo> TiposEquipos { get; set; }
    public DbSet<Equipo> Equipos { get; set; }
    public DbSet<AccesorioEquipo> AccesoriosEquipos { get; set; }
    public DbSet<Sede> Sedes { get; set; }
    public DbSet<Porteria> Porterias { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<RolPermiso> RolesPermisos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    public DbSet<RegistroAcceso> RegistrosAccesos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Clave primaria compuesta
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.IdRol, rp.IdPermiso });

        // Restricciones UNIQUE
        modelBuilder.Entity<Facultad>()
            .HasIndex(f => f.NombreFacultad).IsUnique();

        modelBuilder.Entity<Estudiante>()
            .HasIndex(e => e.NumeroDocumento).IsUnique();

        modelBuilder.Entity<Estudiante>()
            .HasIndex(e => e.CorreoElectronico).IsUnique();

        modelBuilder.Entity<MarcaEquipo>()
            .HasIndex(m => m.NombreMarca).IsUnique();

        modelBuilder.Entity<TipoEquipo>()
            .HasIndex(t => t.NombreTipo).IsUnique();

        modelBuilder.Entity<Equipo>()
            .HasIndex(eq => eq.Serial).IsUnique();

        modelBuilder.Entity<Rol>()
            .HasIndex(r => r.NombreRol).IsUnique();

        modelBuilder.Entity<Permiso>()
            .HasIndex(p => p.NombrePermiso).IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.NombreUsuario).IsUnique();

        // Evitar borrado en cascada cíclico en accesos
        modelBuilder.Entity<RegistroAcceso>()
            .HasOne(ra => ra.Guardia)
            .WithMany(u => u.RegistrosAccesosGuardia)
            .HasForeignKey(ra => ra.IdUsuarioGuardia)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegistroAcceso>()
            .HasOne(ra => ra.Estudiante)
            .WithMany(e => e.RegistrosAccesos)
            .HasForeignKey(ra => ra.IdEstudiante)
            .OnDelete(DeleteBehavior.Restrict);
    }
}