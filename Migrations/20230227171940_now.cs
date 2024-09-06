using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class now : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SpecificationModels_ParentId",
                table: "SpecificationModels",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels",
                column: "ParentId",
                principalTable: "SpecificationModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationModels_ParentId",
                table: "SpecificationModels");
        }
    }
}
