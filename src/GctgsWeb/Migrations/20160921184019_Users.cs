using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GctgsWeb.Migrations
{
    public partial class Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Crsid = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "BoardGames",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_OwnerId",
                table: "BoardGames",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGames_Users_OwnerId",
                table: "BoardGames",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGames_Users_OwnerId",
                table: "BoardGames");

            migrationBuilder.DropIndex(
                name: "IX_BoardGames_OwnerId",
                table: "BoardGames");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "BoardGames");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
