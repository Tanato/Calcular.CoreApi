using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Calcular.CoreApi.Migrations
{
    public partial class v013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"CREATE FUNCTION [dbo].TotalHonorarios ( @ProcessoID INT )
                        RETURNS MONEY WITH SCHEMABINDING
                        BEGIN
                        DECLARE @Result MONEY;
                        SELECT @Result = SUM(CASE WHEN Registro = 0 AND Cancelado = 0 THEN Valor ELSE 0 END) - 
                                         SUM(CASE WHEN Registro = 1 AND Cancelado = 0 THEN Valor ELSE 0 END)
                        FROM [dbo].Honorarios WHERE ProcessoId = @ProcessoID; 
                        RETURN @Result END";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function [dbo].TotalHonorarios");
        }
    }
}
