using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WreFigDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchInstructions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BranchInstructions",
                columns: table => new
                {
                    Id            = table.Column<int>(type: "int", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId      = table.Column<int>(type: "int",              nullable: false),
                    Position      = table.Column<int>(type: "int",              nullable: false),
                    Section       = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content       = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHighlighted = table.Column<bool>(type: "bit",             nullable: false),
                    SortOrder     = table.Column<int>(type: "int",              nullable: false),
                    UpdatedAt     = table.Column<DateTime>(type: "datetime2",   nullable: false),
                    UpdatedById   = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchInstructions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchInstructions_BranchId_Position",
                table: "BranchInstructions",
                columns: new[] { "BranchId", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BranchInstructions");
        }
    }
}
