using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reclamada",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "Reclamante",
                table: "Processos");

            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "Processos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroAutores",
                table: "Processos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Reu",
                table: "Processos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autor",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "NumeroAutores",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "Reu",
                table: "Processos");

            migrationBuilder.AddColumn<string>(
                name: "Reclamada",
                table: "Processos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reclamante",
                table: "Processos",
                nullable: true);
        }
    }
}
