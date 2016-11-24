using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Saida",
                table: "Servicos",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Prazo",
                table: "Servicos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Saida",
                table: "Servicos",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Prazo",
                table: "Servicos",
                nullable: false);
        }
    }
}
