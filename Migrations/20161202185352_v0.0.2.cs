using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Calcular.CoreApi.Shared;

namespace Calcular.CoreApi.Migrations
{
    public partial class v002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propostas_Processos_ProcessoId",
                table: "Propostas");

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComoChegouDetalhe",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contato",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContatoId",
                table: "Propostas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Local",
                table: "Propostas",
                nullable: false,
                defaultValue: TipoJusticaEnum.Forum);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProcessoId",
                table: "Propostas",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataProposta",
                table: "Propostas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_UsuarioId",
                table: "Propostas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Propostas_Processos_ProcessoId",
                table: "Propostas",
                column: "ProcessoId",
                principalTable: "Processos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Propostas_Users_UsuarioId",
                table: "Propostas",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propostas_Processos_ProcessoId",
                table: "Propostas");

            migrationBuilder.DropForeignKey(
                name: "FK_Propostas_Users_UsuarioId",
                table: "Propostas");

            migrationBuilder.DropIndex(
                name: "IX_Propostas_UsuarioId",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "ComoChegouDetalhe",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Contato",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "ContatoId",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Local",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Propostas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Propostas");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessoId",
                table: "Propostas",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataProposta",
                table: "Propostas",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Propostas_Processos_ProcessoId",
                table: "Propostas",
                column: "ProcessoId",
                principalTable: "Processos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
