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
                name: "Advogado",
                table: "Processos");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Processos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorario",
                table: "Processos",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Advogado",
                table: "Processos",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorario",
                table: "Processos",
                nullable: false);
        }
    }
}
