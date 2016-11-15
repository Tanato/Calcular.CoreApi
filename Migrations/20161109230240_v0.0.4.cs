using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processos_Clientes_ClienteId",
                table: "Processos");

            migrationBuilder.DropIndex(
                name: "IX_Processos_ClienteId",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Processos");

            migrationBuilder.AddColumn<int>(
                name: "AdvogadoId",
                table: "Processos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Processos_AdvogadoId",
                table: "Processos",
                column: "AdvogadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processos_Clientes_AdvogadoId",
                table: "Processos",
                column: "AdvogadoId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processos_Clientes_AdvogadoId",
                table: "Processos");

            migrationBuilder.DropIndex(
                name: "IX_Processos_AdvogadoId",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "AdvogadoId",
                table: "Processos");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Processos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Processos_ClienteId",
                table: "Processos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processos_Clientes_ClienteId",
                table: "Processos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
