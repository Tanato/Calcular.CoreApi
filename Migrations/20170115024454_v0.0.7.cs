using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Calcular.CoreApi.Migrations
{
    public partial class v007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comissoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    HoraMaxima = table.Column<decimal>(nullable: false),
                    HoraMinima = table.Column<decimal>(nullable: false),
                    TipoAtividadeId = table.Column<int>(nullable: false),
                    Valor = table.Column<decimal>(nullable: false),
                    Vigencia = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comissoes_TipoAtividades_TipoAtividadeId",
                        column: x => x.TipoAtividadeId,
                        principalTable: "TipoAtividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comissoes_TipoAtividadeId",
                table: "Comissoes",
                column: "TipoAtividadeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comissoes");
        }
    }
}
