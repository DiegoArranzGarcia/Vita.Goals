using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vita.Persistance.Sql.Migrations
{
    public partial class NewGoalStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GoalStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "ToBeDefined");

            migrationBuilder.UpdateData(
                table: "GoalStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Ready");

            migrationBuilder.InsertData(
                table: "GoalStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "InProgress" },
                    { 4, "Completed" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GoalStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GoalStatus",
                keyColumn: "Id",
                keyValue: 4);

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
    }
}
