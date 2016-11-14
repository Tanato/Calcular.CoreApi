using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processos_Users_PeritoId1",
                table: "Processos");

            migrationBuilder.DropIndex(
                name: "IX_Processos_PeritoId1",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "PeritoId",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "PeritoId1",
                table: "Processos");

            migrationBuilder.AddColumn<int>(
                name: "IndicacaoId",
                table: "Processos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndicacaoId1",
                table: "Processos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processos_IndicacaoId1",
                table: "Processos",
                column: "IndicacaoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Processos_Users_IndicacaoId1",
                table: "Processos",
                column: "IndicacaoId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processos_Users_IndicacaoId1",
                table: "Processos");

            migrationBuilder.DropIndex(
                name: "IX_Processos_IndicacaoId1",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "IndicacaoId",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "IndicacaoId1",
                table: "Processos");

            migrationBuilder.AddColumn<int>(
                name: "PeritoId",
                table: "Processos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PeritoId1",
                table: "Processos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processos_PeritoId1",
                table: "Processos",
                column: "PeritoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Processos_Users_PeritoId1",
                table: "Processos",
                column: "PeritoId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
