﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeGolf.Persistence.Migrations
{
    public partial class real_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RealName",
                table: "Users",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealName",
                table: "Users");
        }
    }
}
