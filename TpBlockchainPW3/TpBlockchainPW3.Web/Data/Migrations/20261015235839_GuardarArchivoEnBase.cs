using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TpBlockchainPW3.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class GuardarArchivoEnBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "Documentos");

            migrationBuilder.AddColumn<byte[]>(
                name: "Contenido",
                table: "Documentos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "Documentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contenido",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "Documentos");

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "Documentos",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "");
        }
    }
}
