using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TpBlockchainPW3.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    NombreDestinatario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DniDestinatario = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Materia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Nota = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    RutaArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    HashSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmisorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_AspNetUsers_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_Codigo",
                table: "Documentos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_EmisorId",
                table: "Documentos",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_HashSha256",
                table: "Documentos",
                column: "HashSha256",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");
        }
    }
}
