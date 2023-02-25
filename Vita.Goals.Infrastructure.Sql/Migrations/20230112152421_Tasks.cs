using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vita.Persistance.Sql.Migrations;

public partial class Tasks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TaskStatus",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaskStatus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tasks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PlannedDate_Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                PlannedDate_End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", nullable: false),
                AssociatedToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                TaskStatusId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tasks", x => x.Id);
                table.ForeignKey(
                    name: "FK_Tasks_Goals_AssociatedToId",
                    column: x => x.AssociatedToId,
                    principalTable: "Goals",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Tasks_TaskStatus_TaskStatusId",
                    column: x => x.TaskStatusId,
                    principalTable: "TaskStatus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "TaskStatus",
            columns: new[] { "Id", "Name" },
            values: new object[] { 1, "Ready" });

        migrationBuilder.InsertData(
            table: "TaskStatus",
            columns: new[] { "Id", "Name" },
            values: new object[] { 2, "InProgress" });

        migrationBuilder.InsertData(
            table: "TaskStatus",
            columns: new[] { "Id", "Name" },
            values: new object[] { 3, "Completed" });

        migrationBuilder.CreateIndex(
            name: "IX_Tasks_AssociatedToId",
            table: "Tasks",
            column: "AssociatedToId");

        migrationBuilder.CreateIndex(
            name: "IX_Tasks_TaskStatusId",
            table: "Tasks",
            column: "TaskStatusId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Tasks");

        migrationBuilder.DropTable(
            name: "TaskStatus");
    }
}
