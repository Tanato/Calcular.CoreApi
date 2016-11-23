using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Atividades");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Atividades",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Perfil",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Nascimento",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorarios",
                table: "Clientes",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ComoChegou",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Atividades");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Atividades",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Perfil",
                table: "Clientes",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Nascimento",
                table: "Clientes",
                nullable: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorarios",
                table: "Clientes",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "ComoChegou",
                table: "Clientes",
                nullable: false);
        }
    }
}
