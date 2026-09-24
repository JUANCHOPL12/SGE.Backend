using Microsoft.EntityFrameworkCore;
using SGE.Backend.Models;
using BCrypt.Net;

namespace SGE.Backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // 1. Asegurar la creación de los Roles base si la tabla está vacía
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Rol>
                {
                    new Rol { NombreRol = "SuperAdmin" },
                    new Rol { NombreRol = "Administrador" },
                    new Rol { NombreRol = "Guardia" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // 2. Obtener el ID del Rol SuperAdmin
            var superAdminRol = await context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "SuperAdmin");

            if (superAdminRol != null)
            {
                // Buscar si ya existe el usuario 'admin'
                var usuarioExistente = await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == "admin");

                // AQUÍ ESTÁ EL CAMBIO: Ponemos exactamente la contraseña Cun123456*
                string hashValido = BCrypt.Net.BCrypt.HashPassword("Cun123456*");

                if (usuarioExistente == null)
                {
                    var superAdmin = new Usuario
                    {
                        NombreUsuario = "admin",
                        PasswordHash = hashValido,
                        NombreCompleto = "Administrador Principal",
                        Estado = "Activo",
                        IdRol = superAdminRol.IdRol
                    };

                    await context.Usuarios.AddAsync(superAdmin);
                }
                else
                {
                    // Si ya existe en SQL, le actualizamos el Hash a la nueva clave
                    usuarioExistente.PasswordHash = hashValido;
                    usuarioExistente.Estado = "Activo";
                    context.Usuarios.Update(usuarioExistente);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}