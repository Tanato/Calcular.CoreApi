using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Calcular.CoreApi.Migrations
{
    public partial class v0011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoServicos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoServicos", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "TipoServicoId",
                table: "Servicos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropTable(
                name: "TipoServicos");
        }
    }
}
