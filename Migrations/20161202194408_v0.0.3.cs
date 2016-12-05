using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Calcular.CoreApi.Shared;

namespace Calcular.CoreApi.Migrations
{
    public partial class v003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Fechado",
                table: "Propostas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Motivo",
                table: "Propostas",
                nullable: false,
                defaultValue: MotivoEnum.Outro);

            migrationBuilder.AddColumn<string>(
                name: "MovitoDetalhe",
                table: "Propostas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fechado",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "MovitoDetalhe",
                table: "Propostas");
        }
    }
}
