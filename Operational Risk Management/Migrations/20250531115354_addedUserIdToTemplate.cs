using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operational_Risk_Management.Migrations
{
    /// <inheritdoc />
    public partial class addedUserIdToTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FocalPoint",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Templates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Templates_ApplicationUserId",
                table: "Templates",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_ApplicationUsers_ApplicationUserId",
                table: "Templates",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_ApplicationUsers_ApplicationUserId",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_ApplicationUserId",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Templates");

            migrationBuilder.AlterColumn<string>(
                name: "FocalPoint",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
