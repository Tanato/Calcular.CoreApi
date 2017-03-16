using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Calcular.CoreApi.Shared;

namespace Calcular.CoreApi.Migrations
{
    public partial class v011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusHonorario",
                table: "Processos",
                nullable: false,
                defaultValue: StatusHonorarioEnum.Pendente);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusHonorario",
                table: "Processos");
        }
    }
}
