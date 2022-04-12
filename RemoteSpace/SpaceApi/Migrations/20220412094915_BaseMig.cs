using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpaceApi.Migrations
{
    public partial class BaseMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EleFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    Owner = table.Column<string>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Shared = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDirectory = table.Column<bool>(type: "INTEGER", nullable: false),
                    FatherDirectoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EleFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EleFiles_EleFiles_FatherDirectoryId",
                        column: x => x.FatherDirectoryId,
                        principalTable: "EleFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EleFiles_FatherDirectoryId",
                table: "EleFiles",
                column: "FatherDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EleFiles_Name_Owner_FatherDirectoryId_IsDirectory",
                table: "EleFiles",
                columns: new[] { "Name", "Owner", "FatherDirectoryId", "IsDirectory" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EleFiles");
        }
    }
}
