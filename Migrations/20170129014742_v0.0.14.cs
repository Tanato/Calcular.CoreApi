using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v0014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Atividades");

            migrationBuilder.AddColumn<long>(
                name: "TempoTicks",
                table: "Atividades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempoTicks",
                table: "Atividades");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Tempo",
                table: "Atividades",
                nullable: true);
        }
    }
}
