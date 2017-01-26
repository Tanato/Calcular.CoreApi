using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v009 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Honorario",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Fechado",
                table: "Propostas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Honorario",
                table: "Propostas",
                nullable: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Fechado",
                table: "Propostas",
                nullable: false);
        }
    }
}
