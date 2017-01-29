using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v0012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoImpressao",
                table: "Servicos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacaoComissao",
                table: "Atividades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoImpressao",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ObservacaoComissao",
                table: "Atividades");
        }
    }
}
