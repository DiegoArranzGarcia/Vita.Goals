using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Vita.Persistance.Sql.Migrations
{
    public partial class RequiredCreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Categories",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));
        }
    }
}
