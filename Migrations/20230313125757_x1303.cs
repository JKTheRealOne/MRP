using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class x1303 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels",
                column: "ParentId",
                principalTable: "SpecificationModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationModels_SpecificationModels_ParentId",
                table: "SpecificationModels",
                column: "ParentId",
                principalTable: "SpecificationModels",
                principalColumn: "Id");
        }
    }
}
