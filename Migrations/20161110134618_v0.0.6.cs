using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Celular2",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone2",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Celular2",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Telefone2",
                table: "Clientes");
        }
    }
}
