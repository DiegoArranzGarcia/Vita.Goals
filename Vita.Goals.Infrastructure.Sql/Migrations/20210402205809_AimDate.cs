using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Vita.Persistance.Sql.Migrations;

public partial class AimDate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "AimDate_End",
            table: "Goals",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "AimDate_Start",
            table: "Goals",
            type: "datetimeoffset",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AimDate_End",
            table: "Goals");

        migrationBuilder.DropColumn(
            name: "AimDate_Start",
            table: "Goals");
    }
}
