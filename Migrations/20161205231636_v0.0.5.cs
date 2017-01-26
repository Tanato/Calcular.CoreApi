using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovitoDetalhe",
                table: "Propostas");

            migrationBuilder.AddColumn<string>(
                name: "MotivoDetalhe",
                table: "Propostas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoDetalhe",
                table: "Propostas");

            migrationBuilder.AddColumn<string>(
                name: "MovitoDetalhe",
                table: "Propostas",
                nullable: true);
        }
    }
}
