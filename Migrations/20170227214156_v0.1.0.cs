using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Calcular.CoreApi.Shared;

namespace Calcular.CoreApi.Migrations
{
    public partial class v010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FuncionarioId",
                table: "ComissoesFuncionarioMes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ComissoesFuncionarioMes",
                nullable: false,
                defaultValue: StatusApuracaoComissao.Aberto);

            migrationBuilder.CreateIndex(
                name: "IX_ComissoesFuncionarioMes_FuncionarioId",
                table: "ComissoesFuncionarioMes",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComissoesFuncionarioMes_Users_FuncionarioId",
                table: "ComissoesFuncionarioMes",
                column: "FuncionarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComissoesFuncionarioMes_Users_FuncionarioId",
                table: "ComissoesFuncionarioMes");

            migrationBuilder.DropIndex(
                name: "IX_ComissoesFuncionarioMes_FuncionarioId",
                table: "ComissoesFuncionarioMes");

            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "ComissoesFuncionarioMes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ComissoesFuncionarioMes");
        }
    }
}
