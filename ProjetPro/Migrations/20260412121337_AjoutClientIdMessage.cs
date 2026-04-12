using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetPro.Migrations
{
    /// <inheritdoc />
    public partial class AjoutClientIdMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoldeMessage",
                table: "Client",
                newName: "SoldeMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoldeMessages",
                table: "Client",
                newName: "SoldeMessage");
        }
    }
}
