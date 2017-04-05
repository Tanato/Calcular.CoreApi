using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Calcular.CoreApi.Migrations
{
    public partial class v012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseProcessoId",
                table: "Processos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FaseProcessos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaseProcessos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Processos_FaseProcessoId",
                table: "Processos",
                column: "FaseProcessoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processos_FaseProcessos_FaseProcessoId",
                table: "Processos",
                column: "FaseProcessoId",
                principalTable: "FaseProcessos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processos_FaseProcessos_FaseProcessoId",
                table: "Processos");

            migrationBuilder.DropTable(
                name: "FaseProcessos");

            migrationBuilder.DropIndex(
                name: "IX_Processos_FaseProcessoId",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "FaseProcessoId",
                table: "Processos");
        }
    }
}
