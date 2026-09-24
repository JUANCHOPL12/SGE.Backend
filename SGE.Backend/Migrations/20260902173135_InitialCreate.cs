using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGE.Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facultades",
                columns: table => new
                {
                    id_facultad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_facultad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facultades", x => x.id_facultad);
                });

            migrationBuilder.CreateTable(
                name: "marcas_equipos",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas_equipos", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "permisos",
                columns: table => new
                {
                    id_permiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_permiso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permisos", x => x.id_permiso);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_rol = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "sedes",
                columns: table => new
                {
                    id_sede = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_sede = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sedes", x => x.id_sede);
                });

            migrationBuilder.CreateTable(
                name: "tipos_equipos",
                columns: table => new
                {
                    id_tipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_equipos", x => x.id_tipo);
                });

            migrationBuilder.CreateTable(
                name: "carreras",
                columns: table => new
                {
                    id_carrera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_facultad = table.Column<int>(type: "int", nullable: false),
                    nombre_carrera = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carreras", x => x.id_carrera);
                    table.ForeignKey(
                        name: "FK_carreras_facultades_id_facultad",
                        column: x => x.id_facultad,
                        principalTable: "facultades",
                        principalColumn: "id_facultad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles_permisos",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    id_permiso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles_permisos", x => new { x.id_rol, x.id_permiso });
                    table.ForeignKey(
                        name: "FK_roles_permisos_permisos_id_permiso",
                        column: x => x.id_permiso,
                        principalTable: "permisos",
                        principalColumn: "id_permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roles_permisos_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_completo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    usuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "porterias",
                columns: table => new
                {
                    id_porteria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_sede = table.Column<int>(type: "int", nullable: false),
                    nombre_porteria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_porterias", x => x.id_porteria);
                    table.ForeignKey(
                        name: "FK_porterias_sedes_id_sede",
                        column: x => x.id_sede,
                        principalTable: "sedes",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estudiantes",
                columns: table => new
                {
                    id_estudiante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    correo_electronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_carrera = table.Column<int>(type: "int", nullable: false),
                    foto_perfil_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estudiantes", x => x.id_estudiante);
                    table.ForeignKey(
                        name: "FK_estudiantes_carreras_id_carrera",
                        column: x => x.id_carrera,
                        principalTable: "carreras",
                        principalColumn: "id_carrera",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "log_auditoria",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    accion_realizada = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_auditoria", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_log_auditoria_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipos",
                columns: table => new
                {
                    id_equipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estudiante = table.Column<int>(type: "int", nullable: false),
                    id_tipo = table.Column<int>(type: "int", nullable: false),
                    id_marca = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    foto_equipo_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipos", x => x.id_equipo);
                    table.ForeignKey(
                        name: "FK_equipos_estudiantes_id_estudiante",
                        column: x => x.id_estudiante,
                        principalTable: "estudiantes",
                        principalColumn: "id_estudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_equipos_marcas_equipos_id_marca",
                        column: x => x.id_marca,
                        principalTable: "marcas_equipos",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_equipos_tipos_equipos_id_tipo",
                        column: x => x.id_tipo,
                        principalTable: "tipos_equipos",
                        principalColumn: "id_tipo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accesorios_equipo",
                columns: table => new
                {
                    id_accesorio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_equipo = table.Column<int>(type: "int", nullable: false),
                    descripcion_accesorio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    foto_accesorios_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accesorios_equipo", x => x.id_accesorio);
                    table.ForeignKey(
                        name: "FK_accesorios_equipo_equipos_id_equipo",
                        column: x => x.id_equipo,
                        principalTable: "equipos",
                        principalColumn: "id_equipo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registro_accesos",
                columns: table => new
                {
                    id_registro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estudiante = table.Column<int>(type: "int", nullable: false),
                    id_equipo = table.Column<int>(type: "int", nullable: true),
                    id_usuario_guardia = table.Column<int>(type: "int", nullable: false),
                    id_porteria = table.Column<int>(type: "int", nullable: false),
                    tipo_movimiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_accesos", x => x.id_registro);
                    table.ForeignKey(
                        name: "FK_registro_accesos_equipos_id_equipo",
                        column: x => x.id_equipo,
                        principalTable: "equipos",
                        principalColumn: "id_equipo");
                    table.ForeignKey(
                        name: "FK_registro_accesos_estudiantes_id_estudiante",
                        column: x => x.id_estudiante,
                        principalTable: "estudiantes",
                        principalColumn: "id_estudiante",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registro_accesos_porterias_id_porteria",
                        column: x => x.id_porteria,
                        principalTable: "porterias",
                        principalColumn: "id_porteria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registro_accesos_usuarios_id_usuario_guardia",
                        column: x => x.id_usuario_guardia,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accesorios_equipo_id_equipo",
                table: "accesorios_equipo",
                column: "id_equipo");

            migrationBuilder.CreateIndex(
                name: "IX_carreras_id_facultad",
                table: "carreras",
                column: "id_facultad");

            migrationBuilder.CreateIndex(
                name: "IX_equipos_id_estudiante",
                table: "equipos",
                column: "id_estudiante");

            migrationBuilder.CreateIndex(
                name: "IX_equipos_id_marca",
                table: "equipos",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_equipos_id_tipo",
                table: "equipos",
                column: "id_tipo");

            migrationBuilder.CreateIndex(
                name: "IX_equipos_serial",
                table: "equipos",
                column: "serial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_correo_electronico",
                table: "estudiantes",
                column: "correo_electronico",
                unique: true,
                filter: "[correo_electronico] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_id_carrera",
                table: "estudiantes",
                column: "id_carrera");

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_numero_documento",
                table: "estudiantes",
                column: "numero_documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facultades_nombre_facultad",
                table: "facultades",
                column: "nombre_facultad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_id_usuario",
                table: "log_auditoria",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_marcas_equipos_nombre_marca",
                table: "marcas_equipos",
                column: "nombre_marca",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permisos_nombre_permiso",
                table: "permisos",
                column: "nombre_permiso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_porterias_id_sede",
                table: "porterias",
                column: "id_sede");

            migrationBuilder.CreateIndex(
                name: "IX_registro_accesos_id_equipo",
                table: "registro_accesos",
                column: "id_equipo");

            migrationBuilder.CreateIndex(
                name: "IX_registro_accesos_id_estudiante",
                table: "registro_accesos",
                column: "id_estudiante");

            migrationBuilder.CreateIndex(
                name: "IX_registro_accesos_id_porteria",
                table: "registro_accesos",
                column: "id_porteria");

            migrationBuilder.CreateIndex(
                name: "IX_registro_accesos_id_usuario_guardia",
                table: "registro_accesos",
                column: "id_usuario_guardia");

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre_rol",
                table: "roles",
                column: "nombre_rol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_permisos_id_permiso",
                table: "roles_permisos",
                column: "id_permiso");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_equipos_nombre_tipo",
                table: "tipos_equipos",
                column: "nombre_tipo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_usuario",
                table: "usuarios",
                column: "usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accesorios_equipo");

            migrationBuilder.DropTable(
                name: "log_auditoria");

            migrationBuilder.DropTable(
                name: "registro_accesos");

            migrationBuilder.DropTable(
                name: "roles_permisos");

            migrationBuilder.DropTable(
                name: "equipos");

            migrationBuilder.DropTable(
                name: "porterias");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropTable(
                name: "estudiantes");

            migrationBuilder.DropTable(
                name: "marcas_equipos");

            migrationBuilder.DropTable(
                name: "tipos_equipos");

            migrationBuilder.DropTable(
                name: "sedes");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "carreras");

            migrationBuilder.DropTable(
                name: "facultades");
        }
    }
}
