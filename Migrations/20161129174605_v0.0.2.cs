using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AtividadeId",
                table: "Atividades",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoExecucao",
                table: "Atividades",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TipoAtividades",
                nullable: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TipoAtividades_Nome",
                table: "TipoAtividades",
                column: "Nome");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TipoAtividades_Nome",
                table: "TipoAtividades");

            migrationBuilder.DropColumn(
                name: "AtividadeId",
                table: "Atividades");

            migrationBuilder.DropColumn(
                name: "TipoExecucao",
                table: "Atividades");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TipoAtividades",
                nullable: true);
        }
    }
}
