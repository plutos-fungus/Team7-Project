using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfsUp.Migrations
{
    public partial class InitaialCreateV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Surfboard",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Surfboard",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Surfboard_IdentityUserId",
                table: "Surfboard",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Surfboard_AspNetUsers_IdentityUserId",
                table: "Surfboard",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Surfboard_AspNetUsers_IdentityUserId",
                table: "Surfboard");

            migrationBuilder.DropIndex(
                name: "IX_Surfboard_IdentityUserId",
                table: "Surfboard");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Surfboard");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Surfboard");
        }
    }
}
