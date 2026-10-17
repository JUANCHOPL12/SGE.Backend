using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SGE.Backend.Data;
using SGE.Backend.Services;

namespace SGE.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Registro del DbContext para conectar con SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // =========================================================================
            // 2. CONFIGURACIÓN DE CORS (Solución al error Cross-Origin en Blazor WASM)
            // =========================================================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorApp", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5232",  // Puerto actual de tu Frontend Blazor
                            "https://localhost:7243",
                            "https://localhost:7000"
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // 3. Registro de Inyección de Dependencias
            builder.Services.AddScoped<IAuthService, AuthService>();

            // 4. Controladores
            builder.Services.AddControllers();

            // 5. Configuración del Esquema de Autenticación JWT Bearer (RNF-08)
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Clave JWT no configurada.");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // 6. Configuración de Swagger UI con soporte para el botón "Authorize"
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGE Backend API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Ingrese el token JWT en este formato: Bearer {tu_token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // 7. Inicialización del DbSeeder (Genera datos iniciales como el usuario admin)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await DbSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al ejecutar el DbSeeder.");
                }
            }

            // 8. Middleware de Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGE Backend API v1");
                c.RoutePrefix = string.Empty; // Carga Swagger en la raíz
            });

            app.UseHttpsRedirection();

            // =========================================================================
            // 9. HABILITAR PIPELINE DE SEGURIDAD Y RED (ORDEN STRICTO)
            // =========================================================================
            app.UseRouting();

            // ⚠️ IMPORTANTE: UseCors debe invocarse estrictamente ENTRE UseRouting y UseAuthentication
            app.UseCors("AllowBlazorApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}