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
                name: "Nome",
                table: "Atividades");

            migrationBuilder.AlterColumn<int>(
                name: "TipoExecucao",
                table: "Atividades",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Atividades",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipoExecucao",
                table: "Atividades",
                nullable: true);
        }
    }
}
