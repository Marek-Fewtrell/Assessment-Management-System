using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Assessment_Management_System.Data.Migrations
{
    public partial class AssessmentUserRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "teacherID",
                table: "Assessment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_teacherID",
                table: "Assessment",
                column: "teacherID");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessment_AspNetUsers_teacherID",
                table: "Assessment",
                column: "teacherID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessment_AspNetUsers_teacherID",
                table: "Assessment");

            migrationBuilder.DropIndex(
                name: "IX_Assessment_teacherID",
                table: "Assessment");

            migrationBuilder.DropColumn(
                name: "teacherID",
                table: "Assessment");
        }
    }
}
