using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Calcular.CoreApi.Migrations
{
    public partial class v0017 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraMaxima",
                table: "Comissoes");

            migrationBuilder.DropColumn(
                name: "HoraMinima",
                table: "Comissoes");

            migrationBuilder.CreateTable(
                name: "ComissoesFuncionarioMes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alteracao = table.Column<DateTime>(nullable: false),
                    Ano = table.Column<int>(nullable: false),
                    Mes = table.Column<int>(nullable: false),
                    ResponsavelId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComissoesFuncionarioMes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComissoesFuncionarioMes_Users_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComissaoAtividades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtividadeId = table.Column<int>(nullable: false),
                    ComissaoFuncionarioMesId = table.Column<int>(nullable: false),
                    ValorAdicional = table.Column<decimal>(nullable: true),
                    ValorBase = table.Column<decimal>(nullable: true),
                    ValorFinal = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComissaoAtividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComissaoAtividades_Atividades_AtividadeId",
                        column: x => x.AtividadeId,
                        principalTable: "Atividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComissaoAtividades_ComissoesFuncionarioMes_ComissaoFuncionarioMesId",
                        column: x => x.ComissaoFuncionarioMesId,
                        principalTable: "ComissoesFuncionarioMes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<long>(
                name: "HoraMaxTicks",
                table: "Comissoes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HoraMinTicks",
                table: "Comissoes",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Honorarios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComissaoAtividades_AtividadeId",
                table: "ComissaoAtividades",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ComissaoAtividades_ComissaoFuncionarioMesId",
                table: "ComissaoAtividades",
                column: "ComissaoFuncionarioMesId");

            migrationBuilder.CreateIndex(
                name: "IX_ComissoesFuncionarioMes_ResponsavelId",
                table: "ComissoesFuncionarioMes",
                column: "ResponsavelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraMaxTicks",
                table: "Comissoes");

            migrationBuilder.DropColumn(
                name: "HoraMinTicks",
                table: "Comissoes");

            migrationBuilder.DropTable(
                name: "ComissaoAtividades");

            migrationBuilder.DropTable(
                name: "ComissoesFuncionarioMes");

            migrationBuilder.AddColumn<decimal>(
                name: "HoraMaxima",
                table: "Comissoes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HoraMinima",
                table: "Comissoes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Honorarios",
                nullable: false);
        }
    }
}
