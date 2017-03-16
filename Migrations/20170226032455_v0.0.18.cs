using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v0018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComissaoAtividades_AtividadeId",
                table: "ComissaoAtividades");

            migrationBuilder.CreateIndex(
                name: "IX_ComissaoAtividades_AtividadeId",
                table: "ComissaoAtividades",
                column: "AtividadeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComissaoAtividades_AtividadeId",
                table: "ComissaoAtividades");

            migrationBuilder.CreateIndex(
                name: "IX_ComissaoAtividades_AtividadeId",
                table: "ComissaoAtividades",
                column: "AtividadeId");
        }
    }
}
