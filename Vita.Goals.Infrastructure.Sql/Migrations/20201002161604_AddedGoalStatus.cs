using Microsoft.EntityFrameworkCore.Migrations;

namespace Vita.Persistance.Sql.Migrations
{
    public partial class AddedGoalStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Goals",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<int>(
                name: "GoalStatusId",
                table: "Goals",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "GoalStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GoalStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "todo" });

            migrationBuilder.InsertData(
                table: "GoalStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "completed" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalStatusId",
                table: "Goals",
                column: "GoalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_GoalStatus_GoalStatusId",
                table: "Goals",
                column: "GoalStatusId",
                principalTable: "GoalStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_GoalStatus_GoalStatusId",
                table: "Goals");

            migrationBuilder.DropTable(
                name: "GoalStatus");

            migrationBuilder.DropIndex(
                name: "IX_Goals_GoalStatusId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "GoalStatusId",
                table: "Goals");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Goals",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }
    }
}
