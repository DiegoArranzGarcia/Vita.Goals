using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Vita.Persistance.Sql.Migrations
{
    public partial class FixedCreatedOnLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Goals",
                type: "datetimeoffset(0)",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldMaxLength: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Goals",
                type: "datetimeoffset",
                maxLength: 0,
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset(0)");
        }
    }
}
