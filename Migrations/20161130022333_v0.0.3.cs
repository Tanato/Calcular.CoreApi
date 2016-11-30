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
                name: "AtividadeId",
                table: "Atividades");

            migrationBuilder.AddColumn<int>(
                name: "AtividadeOrigemId",
                table: "Atividades",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_AtividadeOrigemId",
                table: "Atividades",
                column: "AtividadeOrigemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Atividades_Atividades_AtividadeOrigemId",
                table: "Atividades",
                column: "AtividadeOrigemId",
                principalTable: "Atividades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atividades_Atividades_AtividadeOrigemId",
                table: "Atividades");

            migrationBuilder.DropIndex(
                name: "IX_Atividades_AtividadeOrigemId",
                table: "Atividades");

            migrationBuilder.DropColumn(
                name: "AtividadeOrigemId",
                table: "Atividades");

            migrationBuilder.AddColumn<int>(
                name: "AtividadeId",
                table: "Atividades",
                nullable: true);
        }
    }
}
