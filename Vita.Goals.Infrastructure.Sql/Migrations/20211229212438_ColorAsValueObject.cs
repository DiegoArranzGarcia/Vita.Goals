using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vita.Persistance.Sql.Migrations;

public partial class ColorAsValueObject : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "GoalStatus",
            keyColumn: "Id",
            keyValue: 1,
            column: "Name",
            value: "ToDo");

        migrationBuilder.UpdateData(
            table: "GoalStatus",
            keyColumn: "Id",
            keyValue: 2,
            column: "Name",
            value: "Completed");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "GoalStatus",
            keyColumn: "Id",
            keyValue: 1,
            column: "Name",
            value: "todo");

        migrationBuilder.UpdateData(
            table: "GoalStatus",
            keyColumn: "Id",
            keyValue: 2,
            column: "Name",
            value: "completed");
    }
}
